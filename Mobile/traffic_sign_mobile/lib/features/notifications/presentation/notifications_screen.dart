import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../l10n/app_localizations.dart';
import '../application/notification_controller.dart';

class NotificationsScreen extends ConsumerWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final notificationsAsync = ref.watch(notificationControllerProvider);
    final formatter = DateFormat('dd/MM HH:mm');

    return Scaffold(
      body: notificationsAsync.when(
        data: (items) {
          if (items.isEmpty) {
            return Center(child: Text(AppLocalizations.of(context)!.noNotifications));
          }
          return ListView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: items.length,
            itemBuilder: (context, index) {
              final notification = items[index];
              return ListTile(
                leading: Icon(
                  notification.isRead
                      ? Icons.notifications_outlined
                      : Icons.notifications_active,
                  color: notification.isRead
                      ? Theme.of(context).iconTheme.color
                      : Theme.of(context).colorScheme.primary,
                ),
                title: Text(notification.title),
                subtitle: Text(
                  '${notification.message}\n${formatter.format(notification.createdAt)}',
                ),
                isThreeLine: true,
              );
            },
          );
        },
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (err, stack) {
          final l10n = AppLocalizations.of(context)!;
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(l10n.cannotLoadNotifications(err.toString())),
                const SizedBox(height: 8),
                ElevatedButton(
                  onPressed: () =>
                      ref.read(notificationControllerProvider.notifier).refresh(),
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
