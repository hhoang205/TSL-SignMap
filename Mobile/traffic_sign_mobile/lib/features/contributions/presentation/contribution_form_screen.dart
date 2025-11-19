import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:image_picker/image_picker.dart';

import '../application/user_contributions_controller.dart';

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
  String? _imagePath;
  bool _submitting = false;

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
      setState(() => _imagePath = picked.path);
    }
  }

  Future<void> _getCurrentLocation() async {
    final permission = await Geolocator.requestPermission();
    if (permission == LocationPermission.denied) return;
    final position = await Geolocator.getCurrentPosition();
    setState(() {
      _latitude = position.latitude;
      _longitude = position.longitude;
    });
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _submitting = true);
    await ref
        .read(userContributionsControllerProvider.notifier)
        .submitContribution(
          action: _action,
          type: _typeController.text,
          description: _descriptionController.text,
          latitude: _latitude,
          longitude: _longitude,
          imageUrl: _imagePath, // TODO: Upload to storage
        );
    setState(() => _submitting = false);
    if (mounted) Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Đóng góp mới')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              DropdownButtonFormField<String>(
                value: _action,
                items: const [
                  DropdownMenuItem(value: 'Add', child: Text('Thêm mới')),
                  DropdownMenuItem(value: 'Update', child: Text('Cập nhật')),
                  DropdownMenuItem(value: 'Delete', child: Text('Xóa')),
                ],
                onChanged: (value) => setState(() => _action = value ?? 'Add'),
                decoration: const InputDecoration(labelText: 'Loại hành động'),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _typeController,
                decoration: const InputDecoration(labelText: 'Loại biển báo'),
                validator: (value) => value == null || value.isEmpty
                    ? 'Vui lòng nhập loại biển báo'
                    : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descriptionController,
                maxLines: 3,
                decoration: const InputDecoration(labelText: 'Mô tả chi tiết'),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: Text(
                      _latitude != null
                          ? 'Vĩ độ: ${_latitude!.toStringAsFixed(4)}'
                          : 'Vĩ độ: -',
                    ),
                  ),
                  Expanded(
                    child: Text(
                      _longitude != null
                          ? 'Kinh độ: ${_longitude!.toStringAsFixed(4)}'
                          : 'Kinh độ: -',
                    ),
                  ),
                  IconButton(
                    onPressed: _getCurrentLocation,
                    icon: const Icon(Icons.my_location),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              if (_imagePath != null)
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Image.file(File(_imagePath!)),
                ),
              ElevatedButton.icon(
                onPressed: _pickImage,
                icon: const Icon(Icons.camera_alt_outlined),
                label: const Text('Chụp ảnh biển báo'),
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: _submitting ? null : _submit,
                child: _submitting
                    ? const CircularProgressIndicator(strokeWidth: 2)
                    : const Text('Gửi đóng góp (tốn 5 coin)'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
