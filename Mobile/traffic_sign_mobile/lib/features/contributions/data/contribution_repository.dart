import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'models/contribution.dart';

class ContributionRepository {
  ContributionRepository(this._apiClient);

  final ApiClient _apiClient;

  Future<List<Contribution>> getUserContributions(int userId) async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '/api/contributions/user/$userId',
    );
    final data = response.data?['data'] as List<dynamic>? ?? [];
    return data
        .map((item) => Contribution.fromJson(item as Map<String, dynamic>))
        .toList();
  }

  Future<List<Contribution>> getPendingContributions() async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '/api/contributions/status/Pending',
    );
    final data = response.data?['data'] as List<dynamic>? ?? [];
    return data
        .map((item) => Contribution.fromJson(item as Map<String, dynamic>))
        .toList();
  }

  Future<Contribution> submitContribution(Map<String, dynamic> payload) async {
    final response = await _apiClient.post<Map<String, dynamic>>(
      '/api/contributions',
      data: payload,
    );
    final data = response.data?['data'] as Map<String, dynamic>;
    return Contribution.fromJson(data);
  }
}

final contributionRepositoryProvider = Provider<ContributionRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return ContributionRepository(apiClient);
});
