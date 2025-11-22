import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/application/auth_controller.dart';
import '../application/pending_contributions_controller.dart';
import '../data/vote_repository.dart';

class VotingScreen extends ConsumerWidget {
  const VotingScreen({super.key});

  Future<void> _vote(
    WidgetRef ref, {
    required int contributionId,
    required bool isUpvote,
  }) async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;
    await ref
        .read(voteRepositoryProvider)
        .submitVote(
          contributionId: contributionId,
          userId: user.id,
          isUpvote: isUpvote,
          weight: 1 + user.reputation / 100,
        );
    await ref.read(pendingContributionsControllerProvider.notifier).refresh();
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final pendingAsync = ref.watch(pendingContributionsControllerProvider);

    return Scaffold(
      body: pendingAsync.when(
        data: (items) {
          if (items.isEmpty) {
            return const Center(
              child: Text('Không có đóng góp nào chờ duyệt.'),
            );
          }
          return ListView.separated(
            padding: const EdgeInsets.all(16),
            itemCount: items.length,
            separatorBuilder: (_, __) => const SizedBox(height: 12),
            itemBuilder: (context, index) {
              final contribution = items[index];
              return Card(
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        contribution.description,
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      if (contribution.type != null)
                        Text('Loại biển báo: ${contribution.type}'),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          Expanded(
                            child: ElevatedButton.icon(
                              onPressed: () => _vote(
                                ref,
                                contributionId: contribution.id,
                                isUpvote: true,
                              ),
                              icon: const Icon(Icons.thumb_up_alt_outlined),
                              label: const Text('Đồng ý'),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: ElevatedButton.icon(
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Theme.of(
                                  context,
                                ).colorScheme.errorContainer,
                                foregroundColor: Theme.of(
                                  context,
                                ).colorScheme.onErrorContainer,
                              ),
                              onPressed: () => _vote(
                                ref,
                                contributionId: contribution.id,
                                isUpvote: false,
                              ),
                              icon: const Icon(Icons.thumb_down_alt_outlined),
                              label: const Text('Không đồng ý'),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              );
            },
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) =>
            Center(child: Text('Không thể tải dữ liệu: $err')),
      ),
    );
  }
}
