import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/application/auth_controller.dart';
import '../data/contribution_repository.dart';
import '../data/models/contribution.dart';

class UserContributionsController
    extends AutoDisposeAsyncNotifier<List<Contribution>> {
  @override
  Future<List<Contribution>> build() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return [];
    final repository = ref.read(contributionRepositoryProvider);
    return repository.getUserContributions(user.id);
  }

  Future<void> refresh() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(contributionRepositoryProvider);
      return repository.getUserContributions(user.id);
    });
  }

  Future<void> submitContribution({
    required String action,
    required String? type,
    required String? description,
    required double? latitude,
    required double? longitude,
    required String? imageUrl,
    int? signId,
  }) async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;

    final payload = {
      'userId': user.id,
      'action': action,
      'type': type,
      'description': description,
      'latitude': latitude,
      'longitude': longitude,
      'imageUrl': imageUrl,
      'signId': signId,
    };

    await ref.read(contributionRepositoryProvider).submitContribution(payload);
    await refresh();
  }
}

final userContributionsControllerProvider =
    AutoDisposeAsyncNotifierProvider<
      UserContributionsController,
      List<Contribution>
    >(UserContributionsController.new);
