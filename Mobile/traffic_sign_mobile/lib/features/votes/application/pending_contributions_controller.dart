import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../contributions/data/contribution_repository.dart';
import '../../contributions/data/models/contribution.dart';

class PendingContributionsController
    extends AutoDisposeAsyncNotifier<List<Contribution>> {
  @override
  Future<List<Contribution>> build() async {
    final repository = ref.read(contributionRepositoryProvider);
    return repository.getPendingContributions();
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(contributionRepositoryProvider);
      return repository.getPendingContributions();
    });
  }
}

final pendingContributionsControllerProvider =
    AutoDisposeAsyncNotifierProvider<
      PendingContributionsController,
      List<Contribution>
    >(PendingContributionsController.new);
