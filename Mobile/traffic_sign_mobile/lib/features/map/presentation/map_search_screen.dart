import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';

import '../../auth/application/auth_controller.dart';
import '../application/traffic_sign_controller.dart';
import '../domain/traffic_sign_search_payload.dart';

class MapSearchScreen extends ConsumerStatefulWidget {
  const MapSearchScreen({super.key});

  @override
  ConsumerState<MapSearchScreen> createState() => _MapSearchScreenState();
}

class _MapSearchScreenState extends ConsumerState<MapSearchScreen> {
  final _formKey = GlobalKey<FormState>();
  final _typeController = TextEditingController();
  final _radiusController = TextEditingController(text: '2');
  double? _latitude;
  double? _longitude;
  bool _useAdvancedFilter = false;

  Future<void> _getCurrentLocation() async {
    final permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      final requested = await Geolocator.requestPermission();
      if (requested == LocationPermission.denied) {
        return;
      }
    }

    final position = await Geolocator.getCurrentPosition();
    setState(() {
      _latitude = position.latitude;
      _longitude = position.longitude;
    });
  }

  Future<void> _search() async {
    _formKey.currentState?.save();
    final user = ref.read(authControllerProvider).user;
    final radius = double.tryParse(_radiusController.text);

    final payload = TrafficSignSearchPayload(
      type: _useAdvancedFilter ? _typeController.text.trim() : null,
      latitude: _useAdvancedFilter ? _latitude : null,
      longitude: _useAdvancedFilter ? _longitude : null,
      radiusKm: _useAdvancedFilter ? radius : null,
      userId: user?.id,
    );
    await ref.read(trafficSignControllerProvider.notifier).search(payload);
    if (mounted) Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Tìm kiếm nâng cao')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: Column(
            children: [
              SwitchListTile(
                value: _useAdvancedFilter,
                title: const Text('Bật bộ lọc nâng cao (tốn 1 coin)'),
                onChanged: (value) =>
                    setState(() => _useAdvancedFilter = value),
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _typeController,
                decoration: const InputDecoration(labelText: 'Loại biển báo'),
                enabled: _useAdvancedFilter,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _radiusController,
                decoration: const InputDecoration(labelText: 'Bán kính (km)'),
                keyboardType: TextInputType.number,
                enabled: _useAdvancedFilter,
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: Text(
                      _latitude != null
                          ? 'Vị trí: ${_latitude!.toStringAsFixed(4)}, ${_longitude!.toStringAsFixed(4)}'
                          : 'Chưa chọn vị trí',
                    ),
                  ),
                  TextButton(
                    onPressed: _useAdvancedFilter ? _getCurrentLocation : null,
                    child: const Text('Lấy vị trí'),
                  ),
                ],
              ),
              const Spacer(),
              ElevatedButton(onPressed: _search, child: const Text('Áp dụng')),
            ],
          ),
        ),
      ),
    );
  }
}
