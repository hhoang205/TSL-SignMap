import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:image_picker/image_picker.dart';

import '../../../l10n/app_localizations.dart';
import '../../wallet/application/wallet_controller.dart';
import '../application/user_contributions_controller.dart';
import '../data/ai_detection_repository.dart';
import '../data/models/ai_detection_result.dart';

class ContributionFormScreen extends ConsumerStatefulWidget {
  const ContributionFormScreen({super.key});

  @override
  ConsumerState<ContributionFormScreen> createState() =>
      _ContributionFormScreenState();
}

class _ContributionFormScreenState
    extends ConsumerState<ContributionFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _descriptionController = TextEditingController();
  final _typeController = TextEditingController();
  String _action = 'Add';
  double? _latitude;
  double? _longitude;
  File? _imageFile;
  AIDetectionResult? _aiResult;
  bool _detecting = false;
  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    // Auto-capture location when form opens
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _getCurrentLocation();
    });
  }

  @override
  void dispose() {
    _descriptionController.dispose();
    _typeController.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final picked = await picker.pickImage(source: ImageSource.camera);
    if (picked != null) {
      setState(() {
        _imageFile = File(picked.path);
        _aiResult = null; // Reset AI result when new image is picked
      });
      // Auto-detect signs when image is captured
      await _detectSigns();
    }
  }

  Future<void> _detectSigns() async {
    if (_imageFile == null) return;

    setState(() => _detecting = true);
    try {
      final repository = ref.read(aiDetectionRepositoryProvider);
      final result = await repository.detectSigns(_imageFile!);
      setState(() {
        _aiResult = result;
        // Auto-fill type if detected
        if (result.detectedSigns.isNotEmpty) {
          final detectedType = result.detectedSigns.first.type;
          if (detectedType.isNotEmpty && _typeController.text.isEmpty) {
            _typeController.text = detectedType;
          }
        }
      });
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('AI detection failed: $e')),
        );
      }
    } finally {
      setState(() => _detecting = false);
    }
  }

  Future<void> _getCurrentLocation() async {
    try {
      bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Location services are disabled. Please enable them in settings.')),
          );
        }
        return;
      }

      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
        if (permission == LocationPermission.denied) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Location permission denied')),
            );
          }
          return;
        }
      }

      if (permission == LocationPermission.deniedForever) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Location permission permanently denied. Please enable in settings.')),
          );
        }
        return;
      }

      final position = await Geolocator.getCurrentPosition(
        desiredAccuracy: LocationAccuracy.high,
      );
      setState(() {
        _latitude = position.latitude;
        _longitude = position.longitude;
      });
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to get location: $e')),
        );
      }
    }
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    
    final l10n = AppLocalizations.of(context)!;
    
    // Additional validation for "Add" action
    if (_action == 'Add') {
      if (_typeController.text.trim().isEmpty) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(l10n.pleaseEnterSignType)),
        );
        return;
      }
      if (_latitude == null || _longitude == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(l10n.pleaseCaptureLocation)),
        );
        return;
      }
    }
    
    setState(() => _submitting = true);
    try {
      await ref
          .read(userContributionsControllerProvider.notifier)
          .submitContribution(
            action: _action,
            type: _typeController.text.trim().isEmpty ? null : _typeController.text.trim(),
            description: _descriptionController.text.trim().isEmpty 
                ? null 
                : _descriptionController.text.trim(),
            latitude: _latitude,
            longitude: _longitude,
            imageFile: _imageFile,
          );
      
      if (mounted) {
        Navigator.of(context).pop();
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Contribution submitted successfully')),
        );
      }
    } catch (e) {
      if (mounted) {
        // Clear any previous snackbars
        ScaffoldMessenger.of(context).clearSnackBars();
        
        String errorMessage = 'Error submitting contribution';
        
        // Handle DioException to extract server error message
        if (e is DioException) {
          if (e.response != null) {
            final responseData = e.response?.data;
            if (responseData is Map<String, dynamic>) {
              // Try to extract message from server response
              errorMessage = responseData['message'] as String? ?? 
                           responseData['error'] as String? ?? 
                           'Server error: ${e.response?.statusCode}';
            } else {
              errorMessage = 'Server error: ${e.response?.statusCode}';
            }
          } else {
            errorMessage = 'Network error: ${e.message ?? 'Unable to connect to server'}';
          }
        } else {
          // Extract the actual error message, removing "Exception: " prefix if present
          final errorString = e.toString();
          errorMessage = errorString.startsWith('Exception: ') 
              ? errorString.substring(11) 
              : errorString;
        }
        
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(errorMessage),
            backgroundColor: Colors.red,
            duration: const Duration(seconds: 5),
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _submitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Scaffold(
      appBar: AppBar(title: Text(l10n.newContributionTitle)),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              DropdownButtonFormField<String>(
                value: _action,
                items: [
                  DropdownMenuItem(value: 'Add', child: Text(l10n.add)),
                  DropdownMenuItem(value: 'Update', child: Text(l10n.update)),
                  DropdownMenuItem(value: 'Delete', child: Text(l10n.delete)),
                ],
                onChanged: (value) => setState(() => _action = value ?? 'Add'),
                decoration: InputDecoration(labelText: l10n.actionType),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _typeController,
                decoration: InputDecoration(labelText: l10n.signTypeLabel),
                validator: (value) => value == null || value.isEmpty
                    ? l10n.pleaseEnterSignType
                    : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descriptionController,
                maxLines: 3,
                decoration: InputDecoration(labelText: l10n.detailedDescription),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: Text(
                      _latitude != null
                          ? l10n.latitude(_latitude!.toStringAsFixed(4))
                          : l10n.latitude(l10n.dash),
                    ),
                  ),
                  Expanded(
                    child: Text(
                      _longitude != null
                          ? l10n.longitude(_longitude!.toStringAsFixed(4))
                          : l10n.longitude(l10n.dash),
                    ),
                  ),
                  IconButton(
                    onPressed: _getCurrentLocation,
                    icon: const Icon(Icons.my_location),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              if (_imageFile != null)
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.file(_imageFile!),
                    ),
                    if (_detecting)
                      const Padding(
                        padding: EdgeInsets.all(8.0),
                        child: Row(
                          children: [
                            SizedBox(
                              width: 16,
                              height: 16,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            ),
                            SizedBox(width: 8),
                            Text('Detecting signs...'),
                          ],
                        ),
                      ),
                    if (_aiResult != null && _aiResult!.detectedSigns.isNotEmpty)
                      Card(
                        color: Colors.green.shade50,
                        child: Padding(
                          padding: const EdgeInsets.all(12.0),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Icon(Icons.check_circle,
                                      color: Colors.green.shade700),
                                  const SizedBox(width: 8),
                                  Text(
                                    'AI Detection Results',
                                    style: TextStyle(
                                      fontWeight: FontWeight.bold,
                                      color: Colors.green.shade700,
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 8),
                              ..._aiResult!.detectedSigns.map((sign) {
                                return Padding(
                                  padding: const EdgeInsets.only(bottom: 4.0),
                                  child: Text(
                                    '• ${sign.type} (${(sign.confidence * 100).toStringAsFixed(1)}% confidence)',
                                    style: TextStyle(
                                      color: Colors.green.shade900,
                                    ),
                                  ),
                                );
                              }),
                            ],
                          ),
                        ),
                      ),
                    if (_aiResult != null && _aiResult!.detectedSigns.isEmpty)
                      Card(
                        color: Colors.orange.shade50,
                        child: Padding(
                          padding: const EdgeInsets.all(12.0),
                          child: Row(
                            children: [
                              Icon(Icons.warning,
                                  color: Colors.orange.shade700),
                              const SizedBox(width: 8),
                              Expanded(
                                child: Text(
                                  'No signs detected. Please ensure the image contains a traffic sign.',
                                  style: TextStyle(
                                    color: Colors.orange.shade900,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                  ],
                ),
              ElevatedButton.icon(
                onPressed: _pickImage,
                icon: const Icon(Icons.camera_alt_outlined),
                label: Text(l10n.takePhoto),
              ),
              if (_imageFile != null && _aiResult == null && !_detecting)
                ElevatedButton.icon(
                  onPressed: _detectSigns,
                  icon: const Icon(Icons.auto_awesome),
                  label: const Text('Detect Signs with AI'),
                ),
              const SizedBox(height: 24),
              // Display current wallet balance
              Consumer(
                builder: (context, ref, child) {
                  final walletAsync = ref.watch(walletControllerProvider);
                  return walletAsync.when(
                    data: (balance) {
                      if (balance == null) {
                        return const SizedBox.shrink();
                      }
                      final hasEnough = balance.balance >= 5.0;
                      return Card(
                        color: hasEnough ? Colors.green.shade50 : Colors.red.shade50,
                        child: Padding(
                          padding: const EdgeInsets.all(12.0),
                          child: Row(
                            children: [
                              Icon(
                                hasEnough ? Icons.check_circle : Icons.warning,
                                color: hasEnough ? Colors.green.shade700 : Colors.red.shade700,
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                child: Text(
                                  'Số dư hiện tại: ${balance.balance.toStringAsFixed(1)} coin${hasEnough ? ' (Đủ để submit)' : ' (Cần ít nhất 5 coin)'}',
                                  style: TextStyle(
                                    color: hasEnough ? Colors.green.shade900 : Colors.red.shade900,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                    loading: () => const SizedBox.shrink(),
                    error: (error, stack) => const SizedBox.shrink(),
                  );
                },
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _submitting ? null : _submit,
                child: _submitting
                    ? const CircularProgressIndicator(strokeWidth: 2)
                    : Text(l10n.submitContribution),
              ),
              const SizedBox(height: 8),
              Text(
                'Note: Submitting a contribution costs 5 coins.',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Colors.grey.shade600,
                    ),
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
