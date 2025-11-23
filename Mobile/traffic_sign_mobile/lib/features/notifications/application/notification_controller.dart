import 'dart:async';

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
    
    // Fetch notifications first, don't wait for SignalR
    final repository = ref.read(notificationRepositoryProvider);
    final notifications = await repository.getUserNotifications(user.id);
    
    // Initialize SignalR in background (non-blocking)
    _initSignalR().catchError((error) {
      // SignalR connection failure shouldn't block notifications display
      print('SignalR connection failed: $error');
    });
    
    return notifications;
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

    try {
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

      // Add timeout for SignalR connection
      final startFuture = connection.start();
      if (startFuture != null) {
        await startFuture.timeout(
          const Duration(seconds: 5),
          onTimeout: () {
            throw TimeoutException('SignalR connection timeout');
          },
        );
      }
      
      final invokeFuture = connection.invoke('JoinUserGroup', args: [user.id]);
      if (invokeFuture != null) {
        await invokeFuture.timeout(
          const Duration(seconds: 3),
          onTimeout: () {
            throw TimeoutException('SignalR join group timeout');
          },
        );
      }
      
      _connection = connection;
    } catch (e) {
      // Log error but don't throw - SignalR is optional for notifications display
      print('Failed to initialize SignalR: $e');
      _connection = null;
    }
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
