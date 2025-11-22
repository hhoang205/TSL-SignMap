import 'package:dio/dio.dart';

import 'models/geocoding_result.dart';

class OpenStreetMapService {
  OpenStreetMapService({Dio? dio})
      : _dio = dio ??
            Dio(BaseOptions(
              baseUrl: 'https://nominatim.openstreetmap.org',
              headers: {
                'User-Agent': 'TrafficSignApp/1.0',
              },
            ));

  final Dio _dio;

  /// Tìm kiếm địa điểm từ query string
  /// [query] - từ khóa tìm kiếm (ví dụ: "Hà Nội", "Ho Chi Minh City")
  /// [limit] - số lượng kết quả tối đa (mặc định: 10)
  Future<List<GeocodingResult>> searchPlaces(
    String query, {
    int limit = 10,
    String? countryCode,
  }) async {
    try {
      final response = await _dio.get(
        '/search',
        queryParameters: {
          'q': query,
          'format': 'json',
          'limit': limit,
          'addressdetails': 1,
          if (countryCode != null) 'countrycodes': countryCode,
        },
      );

      if (response.data is List) {
        return (response.data as List)
            .map((json) => GeocodingResult.fromJson(json as Map<String, dynamic>))
            .toList();
      }
      return [];
    } catch (e) {
      throw Exception('Lỗi khi tìm kiếm địa điểm: $e');
    }
  }

  /// Reverse geocoding - lấy địa chỉ từ tọa độ
  /// [latitude] - vĩ độ
  /// [longitude] - kinh độ
  Future<GeocodingResult?> reverseGeocode(
    double latitude,
    double longitude,
  ) async {
    try {
      final response = await _dio.get(
        '/reverse',
        queryParameters: {
          'lat': latitude.toString(),
          'lon': longitude.toString(),
          'format': 'json',
          'addressdetails': 1,
        },
      );

      if (response.data is Map<String, dynamic>) {
        return GeocodingResult.fromJson(response.data as Map<String, dynamic>);
      }
      return null;
    } catch (e) {
      throw Exception('Lỗi khi lấy địa chỉ: $e');
    }
  }

  /// Geocoding - lấy tọa độ từ địa chỉ
  /// [address] - địa chỉ cần tìm (ví dụ: "123 Đường ABC, Hà Nội")
  Future<List<GeocodingResult>> geocode(String address) async {
    try {
      final response = await _dio.get(
        '/search',
        queryParameters: {
          'q': address,
          'format': 'json',
          'limit': 5,
          'addressdetails': 1,
        },
      );

      if (response.data is List) {
        return (response.data as List)
            .map((json) => GeocodingResult.fromJson(json as Map<String, dynamic>))
            .toList();
      }
      return [];
    } catch (e) {
      throw Exception('Lỗi khi tìm tọa độ: $e');
    }
  }
}

