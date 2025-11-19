import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart'; // Đảm bảo bạn đang dùng flutter_map bản mới (v6.0 trở lên)
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:latlong2/latlong.dart';
import 'package:http/http.dart' as http;

// Import controller của bạn (giữ nguyên)
import '../application/traffic_sign_controller.dart';

class MapScreen extends ConsumerStatefulWidget {
  const MapScreen({super.key});

  @override
  _MapScreenState createState() => _MapScreenState();
}

class _MapScreenState extends ConsumerState<MapScreen> {
  final TextEditingController _searchController = TextEditingController();
  final MapController _mapController = MapController();

  @override
  Widget build(BuildContext context) {
    final signsAsync = ref.watch(trafficSignControllerProvider);

    // 1. Xử lý Marker riêng biệt, không chặn việc render Map
    List<Marker> markers = [];
    
    // Check trạng thái data để tạo marker
    signsAsync.whenData((signs) {
      markers = signs.map((sign) {
        return Marker(
          point: LatLng(sign.latitude, sign.longitude),
          width: 60,
          height: 60,
          // Sửa child của Marker cho đúng cú pháp flutter_map mới nhất
          child: Tooltip( 
            message: sign.type,
            child: const Icon(
              Icons.location_on,
              color: Colors.redAccent,
              size: 40,
            ),
          ),
        );
      }).toList();
    });

    // Vị trí mặc định (Hà Nội)
    final defaultCenter = LatLng(21.0285, 105.8542);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Biển báo giao thông'),
        actions: [
          IconButton(
            icon: const Icon(Icons.filter_alt),
            onPressed: () => context.push('/home/map/search'),
          ),
          // Nút refresh thủ công
          IconButton(
             icon: const Icon(Icons.refresh),
             onPressed: () => ref.read(trafficSignControllerProvider.notifier).refreshSigns(),
          )
        ],
      ),
      body: Column(
        children: [
          // Search Bar
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _searchController,
                    // GỌI HÀM SEARCH KHI ẤN ENTER
                    onSubmitted: (value) => _searchLocation(value),
                    decoration: InputDecoration(
                      hintText: 'Tìm địa điểm (ví dụ: Hà Nội)...',
                      border: const OutlineInputBorder(),
                      suffixIcon: IconButton(
                        icon: const Icon(Icons.search),
                        onPressed: () => _searchLocation(_searchController.text),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
          
          // Map Area
          Expanded(
            child: Stack(
              children: [
                // LỚP 1: BẢN ĐỒ (Luôn hiển thị)
                FlutterMap(
                  mapController: _mapController,
                  options: MapOptions(
                    initialCenter: defaultCenter, // Syntax v6+
                    initialZoom: 13.0,
                    // Giữ tương tác mượt mà
                    interactionOptions: const InteractionOptions(
                      flags: InteractiveFlag.all,
                    ),
                  ),
                  children: [
                    TileLayer(
                      urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                      userAgentPackageName: 'com.example.traffic_sign_mobile', // QUAN TRỌNG: Thay bằng package id của bạn để tránh bị chặn
                    ),
                    MarkerLayer(markers: markers), // Marker layer nhận list rỗng nếu chưa có data
                  ],
                ),

                // LỚP 2: LOADING HOẶC ERROR (Hiển thị đè lên map nếu cần)
                if (signsAsync.isLoading)
                  const Positioned(
                    top: 20,
                    right: 20,
                    child: Card(
                      child: Padding(
                        padding: EdgeInsets.all(8.0),
                        child: CircularProgressIndicator(strokeWidth: 2),
                      ),
                    ),
                  ),
                
                if (signsAsync.hasError)
                   Positioned(
                    bottom: 20,
                    left: 20,
                    right: 20,
                    child: Card(
                      color: Colors.red.shade100,
                      child: Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Text('Lỗi tải dữ liệu: ${signsAsync.error}'),
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _searchLocation(String query) async {
    if (query.isEmpty) return;
    // Ẩn bàn phím
    FocusManager.instance.primaryFocus?.unfocus();

    final url = Uri.parse(
      'https://nominatim.openstreetmap.org/search?q=$query&format=json&limit=1',
    );

    try {
      final response = await http.get(url, headers: {
        'User-Agent': 'traffic_sign_mobile/1.0', 
      });

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        if (data is List && data.isNotEmpty) {
          final lat = double.parse(data[0]['lat']);
          final lon = double.parse(data[0]['lon']);
          final newCenter = LatLng(lat, lon);

          _mapController.move(newCenter, 15.0);
        } else {
          if(mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Không tìm thấy địa điểm')),
            );
          }
        }
      }
    } catch (e) {
      if(mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Lỗi kết nối: $e')),
        );
      }
    }
  }
}