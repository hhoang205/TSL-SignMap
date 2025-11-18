import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';

import '../application/traffic_sign_controller.dart';

class MapScreen extends ConsumerWidget {
  const MapScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final signsAsync = ref.watch(trafficSignControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Biển báo giao thông'),
        actions: [
          IconButton(
            icon: const Icon(Icons.filter_alt),
            onPressed: () => context.push('/home/map/search'),
          ),
        ],
      ),
      body: signsAsync.when(
        data: (signs) {
          if (signs.isEmpty) {
            return const Center(child: Text('Chưa có dữ liệu biển báo.'));
          }

          final markers = signs
              .map(
                (sign) => Marker(
                  point: LatLng(sign.latitude, sign.longitude),
                  width: 60,
                  height: 60,
                  builder: (context) => Tooltip(
                    message: sign.type,
                    child: const Icon(
                      Icons.location_on,
                      color: Colors.redAccent,
                    ),
                  ),
                ),
              )
              .toList();

          final initialPoint = LatLng(
            signs.first.latitude,
            signs.first.longitude,
          );

          return FlutterMap(
            options: MapOptions(initialCenter: initialPoint, initialZoom: 13),
            children: [
              TileLayer(
                urlTemplate:
                    'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
                subdomains: const ['a', 'b', 'c'],
              ),
              MarkerLayer(markers: markers),
            ],
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text('Không thể tải dữ liệu: $err'),
              const SizedBox(height: 8),
              ElevatedButton(
                onPressed: () => ref
                    .read(trafficSignControllerProvider.notifier)
                    .refreshSigns(),
                child: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
