import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';

import '../../../l10n/app_localizations.dart';

import '../application/geocoding_controller.dart';
import '../application/traffic_sign_controller.dart';
import '../data/models/geocoding_result.dart';
import '../data/models/traffic_sign.dart';
import 'location_search_widget.dart';
import 'map_marker_helper.dart';

class MapScreen extends ConsumerStatefulWidget {
  const MapScreen({super.key});

  @override
  ConsumerState<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends ConsumerState<MapScreen> {
  final MapController _mapController = MapController();
  LatLng? _selectedLocation;

  @override
  void dispose() {
    _mapController.dispose();
    super.dispose();
  }

  void _onLocationSelected(GeocodingResult location) {
    setState(() {
      _selectedLocation = LatLng(location.latitude, location.longitude);
    });
    _mapController.move(_selectedLocation!, 15);
    ref.read(geocodingControllerProvider.notifier).selectLocation(location);
  }

  void _onMarkerTap(LatLng point, TrafficSign? sign) async {
    if (sign != null) {
      // Hiển thị thông tin chi tiết về traffic sign
      await ref
          .read(geocodingControllerProvider.notifier)
          .reverseGeocode(point.latitude, point.longitude);
      
      final geocodingState = ref.read(geocodingControllerProvider);
      if (mounted) {
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: Row(
              children: [
                Icon(
                  MapMarkerHelper.getMarkerIcon(sign.type),
                  color: MapMarkerHelper.getMarkerColor(sign.type),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    sign.type,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
              ],
            ),
            content: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  if (geocodingState.selectedLocation != null) ...[
                    Text(
                      geocodingState.selectedLocation!.displayName,
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                    if (geocodingState.selectedLocation!.address != null) ...[
                      const SizedBox(height: 4),
                      Text(
                        geocodingState.selectedLocation!.address!.formattedAddress,
                        style: TextStyle(color: Colors.grey[600], fontSize: 12),
                      ),
                    ],
                    const SizedBox(height: 12),
                  ],
                  _buildInfoRow(AppLocalizations.of(context)!.status, sign.status),
                  _buildInfoRow(AppLocalizations.of(context)!.validFrom, 
                    '${sign.validFrom.day}/${sign.validFrom.month}/${sign.validFrom.year}'),
                  if (sign.validTo != null)
                    _buildInfoRow(AppLocalizations.of(context)!.validTo, 
                      '${sign.validTo!.day}/${sign.validTo!.month}/${sign.validTo!.year}'),
                  const SizedBox(height: 8),
                  Text(
                    AppLocalizations.of(context)!.coordinates(
                      point.latitude.toStringAsFixed(6),
                      point.longitude.toStringAsFixed(6),
                    ),
                    style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                  ),
                  if (sign.imageUrl != null) ...[
                    const SizedBox(height: 12),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: CachedNetworkImage(
                        imageUrl: sign.imageUrl!,
                        height: 150,
                        fit: BoxFit.cover,
                        placeholder: (context, url) => Container(
                          height: 150,
                          color: Colors.grey[200],
                          child: const Center(
                            child: CircularProgressIndicator(),
                          ),
                        ),
                        errorWidget: (context, url, error) => Container(
                          height: 150,
                          color: Colors.grey[200],
                          child: const Icon(Icons.error),
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: Text(AppLocalizations.of(context)!.close),
              ),
            ],
          ),
        );
      }
    } else {
      // Hiển thị thông tin vị trí thông thường
      await ref
          .read(geocodingControllerProvider.notifier)
          .reverseGeocode(point.latitude, point.longitude);
      
      final geocodingState = ref.read(geocodingControllerProvider);
      if (geocodingState.selectedLocation != null && mounted) {
        final l10n = AppLocalizations.of(context)!;
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: Text(l10n.locationInfo),
            content: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  geocodingState.selectedLocation!.displayName,
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
                if (geocodingState.selectedLocation!.address != null) ...[
                  const SizedBox(height: 8),
                  Text(
                    geocodingState.selectedLocation!.address!.formattedAddress,
                    style: TextStyle(color: Colors.grey[600]),
                  ),
                ],
                const SizedBox(height: 8),
                Text(
                  l10n.coordinates(
                    point.latitude.toStringAsFixed(6),
                    point.longitude.toStringAsFixed(6),
                  ),
                  style: TextStyle(fontSize: 12, color: Colors.grey[600]),
                ),
              ],
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: Text(l10n.close),
              ),
            ],
          ),
        );
      }
    }
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              '$label:',
              style: TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey[700],
                fontSize: 13,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontSize: 13),
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final signsAsync = ref.watch(trafficSignControllerProvider);
    final geocodingState = ref.watch(geocodingControllerProvider);

    return Scaffold(
      body: signsAsync.when(
        data: (signs) {
          final markers = <Marker>[];
    
          // Thêm markers cho biển báo với màu sắc và icon khác nhau theo type
          markers.addAll(
            signs.map<Marker>(
              (sign) {
                final markerColor = MapMarkerHelper.getMarkerColor(sign.type);
                final markerIcon = MapMarkerHelper.getMarkerIcon(sign.type);
                
        return Marker(
          point: LatLng(sign.latitude, sign.longitude),
          width: 60,
          height: 60,
                  child: GestureDetector(
                    onTap: () => _onMarkerTap(
                      LatLng(sign.latitude, sign.longitude),
                      sign,
                    ),
          child: Tooltip( 
            message: sign.type,
                      child: Container(
                        decoration: BoxDecoration(
                          color: Colors.white,
                          shape: BoxShape.circle,
                          boxShadow: [
                            BoxShadow(
                              color: markerColor.withOpacity(0.3),
                              blurRadius: 8,
                              spreadRadius: 2,
                            ),
                          ],
                        ),
                        child: Icon(
                          markerIcon,
                          color: markerColor,
                          size: 36,
                        ),
                      ),
            ),
          ),
        );
              },
            ),
          );

          // Thêm marker cho vị trí được tìm kiếm
          if (_selectedLocation != null) {
            markers.add(
              Marker(
                point: _selectedLocation!,
                width: 60,
                height: 60,
                child: GestureDetector(
                  onTap: () => _onMarkerTap(_selectedLocation!, null),
                  child: Container(
                    decoration: BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                      boxShadow: [
                        BoxShadow(
                          color: Colors.blue.withOpacity(0.3),
                          blurRadius: 8,
                          spreadRadius: 2,
          ),
        ],
      ),
                    child: const Icon(
                      Icons.place,
                      color: Colors.blue,
                      size: 40,
                    ),
                    ),
                  ),
                ),
            );
          }

          final initialPoint = signs.isNotEmpty
              ? LatLng(signs.first.latitude, signs.first.longitude)
              : const LatLng(21.0285, 105.8542); // Hà Nội mặc định

          return Stack(
            children: [
              FlutterMap(
                mapController: _mapController,
                options: MapOptions(
                  initialCenter: _selectedLocation ?? initialPoint,
                  initialZoom: _selectedLocation != null ? 15 : 13,
                  onTap: (tapPosition, point) {
                    // Có thể thêm tính năng click vào map để lấy địa chỉ
                  },
                ),
            children: [
              TileLayer(
                urlTemplate:
                        'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                    userAgentPackageName: 'com.trafficsign.app',
              ),
              MarkerLayer(markers: markers),
                ],
              ),
              Positioned(
                top: 16,
                left: 16,
                right: 16,
                child: LocationSearchWidget(
                  onLocationSelected: _onLocationSelected,
                ),
              ),
            ],
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) {
          final l10n = AppLocalizations.of(context)!;
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(l10n.cannotLoadData(err.toString())),
                const SizedBox(height: 8),
                ElevatedButton(
                  onPressed: () => ref
                      .read(trafficSignControllerProvider.notifier)
                      .refreshSigns(),
                  child: Text(l10n.tryAgain),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
