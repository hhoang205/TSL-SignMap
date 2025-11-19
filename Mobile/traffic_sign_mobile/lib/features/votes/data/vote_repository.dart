import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'vote_summary.dart';

class VoteRepository {
  VoteRepository(this._apiClient);

  final ApiClient _apiClient;

  Future<void> submitVote({
    required int contributionId,
    required int userId,
    required bool isUpvote,
    double weight = 1,
  }) async {
    await _apiClient.post(
      '/api/votes',
      data: {
        'contributionId': contributionId,
        'userId': userId,
        'value': isUpvote,
        'weight': weight,
      },
    );
  }

  Future<VoteSummary> getSummary(int contributionId) async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '/api/votes/contribution/$contributionId/summary',
    );
    return VoteSummary.fromJson(response.data!);
  }
}

final voteRepositoryProvider = Provider<VoteRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return VoteRepository(apiClient);
});
