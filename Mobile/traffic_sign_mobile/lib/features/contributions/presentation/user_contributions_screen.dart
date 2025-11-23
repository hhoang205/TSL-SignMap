import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../l10n/app_localizations.dart';
import '../application/user_contributions_controller.dart';

class UserContributionsScreen extends ConsumerWidget {
  const UserContributionsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final contributionsAsync = ref.watch(userContributionsControllerProvider);

    return Scaffold(
      body: contributionsAsync.when(
        data: (items) {
          if (items.isEmpty) {
            final l10n = AppLocalizations.of(context)!;
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(l10n.noContributions),
                  const SizedBox(height: 12),
                  ElevatedButton(
                    onPressed: () => context.push('/home/contribution/new'),
                    child: Text(l10n.newContribution),
                  ),
                ],
              ),
            );
          }

          final formatter = DateFormat('dd/MM/yyyy HH:mm');
          return RefreshIndicator(
            onRefresh: () => ref
                .read(userContributionsControllerProvider.notifier)
                .refresh(),
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: items.length,
              itemBuilder: (context, index) {
                final contribution = items[index];
                return Card(
                  margin: const EdgeInsets.only(bottom: 12),
                  child: ListTile(
                    title: Text(contribution.description),
                    subtitle: Text(
                      '${contribution.action} | ${contribution.status}\n${formatter.format(contribution.createdAt)}',
                    ),
                    trailing: const Icon(Icons.chevron_right),
                  ),
                );
              },
            ),
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
                      .read(userContributionsControllerProvider.notifier)
                      .refresh(),
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
