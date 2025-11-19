import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'models/traffic_sign.dart';
import '../domain/traffic_sign_search_payload.dart';

class TrafficSignRepository {
  TrafficSignRepository(this._apiClient);

  final ApiClient _apiClient;

  Future<List<TrafficSign>> getAll() async {
    final response = await _apiClient.get<List<dynamic>>('/api/signs');
    final data = response.data ?? [];
    return data
        .map((item) => TrafficSign.fromJson(item as Map<String, dynamic>))
        .toList();
  }

  Future<List<TrafficSign>> search(TrafficSignSearchPayload payload) async {
    final response = await _apiClient.post<Map<String, dynamic>>(
      '/api/signs/search',
      data: payload.toJson(),
    );
    final items = response.data?['data'] as List<dynamic>? ?? [];
    return items
        .map((item) => TrafficSign.fromJson(item as Map<String, dynamic>))
        .toList();
  }
}

final trafficSignRepositoryProvider = Provider<TrafficSignRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return TrafficSignRepository(apiClient);
});
