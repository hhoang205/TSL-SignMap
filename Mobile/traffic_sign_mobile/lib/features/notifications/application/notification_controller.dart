import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:signalr_netcore/hub_connection.dart';
import 'package:signalr_netcore/hub_connection_builder.dart';

import '../../../core/config/app_config.dart';
import '../../auth/application/auth_controller.dart';
import '../data/notification_model.dart';
import '../data/notification_repository.dart';

class NotificationController
    extends AutoDisposeAsyncNotifier<List<AppNotification>> {
  HubConnection? _connection;

  @override
  Future<List<AppNotification>> build() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return [];
    ref.onDispose(_disposeConnection);
    await _initSignalR();
    final repository = ref.read(notificationRepositoryProvider);
    return repository.getUserNotifications(user.id);
  }

  Future<void> refresh() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(notificationRepositoryProvider);
      return repository.getUserNotifications(user.id);
    });
  }

  Future<void> _initSignalR() async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;
    if (_connection != null) return;

    final config = ref.read(appConfigProvider);
    final connection = HubConnectionBuilder()
        .withUrl(config.notificationHubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveNotification', (arguments) {
      if (arguments == null || arguments.isEmpty) return;
      final payload = arguments.first as Map<String, dynamic>;
      final notification = AppNotification.fromJson(payload);
      state = state.whenData((items) => [notification, ...items]);
    });

    await connection.start();
    await connection.invoke('JoinUserGroup', args: [user.id]);
    _connection = connection;
  }

  Future<void> _disposeConnection() async {
    await _connection?.stop();
    _connection = null;
  }
}

final notificationControllerProvider =
    AutoDisposeAsyncNotifierProvider<
      NotificationController,
      List<AppNotification>
    >(NotificationController.new);
