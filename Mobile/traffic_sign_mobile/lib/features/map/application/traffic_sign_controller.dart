import 'dart:async';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:signalr_netcore/hub_connection.dart';
import 'package:signalr_netcore/hub_connection_builder.dart';

import '../../../core/config/app_config.dart';
import '../../auth/application/auth_controller.dart';
import '../data/models/traffic_sign.dart';
import '../data/traffic_sign_repository.dart';
import '../domain/traffic_sign_search_payload.dart';

class TrafficSignController
    extends AutoDisposeAsyncNotifier<List<TrafficSign>> {
  HubConnection? _connection;
  Timer? _refreshTimer;

  @override
  Future<List<TrafficSign>> build() async {
    final repository = ref.read(trafficSignRepositoryProvider);
    final signs = await repository.getAll();
    
    // Setup SignalR connection để listen cho updates
    ref.onDispose(_disposeConnection);
    await _initSignalR();
    
    // Setup periodic refresh mỗi 30 giây như một fallback
    _setupPeriodicRefresh();
    
    return signs;
  }

  Future<void> refreshSigns() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(trafficSignRepositoryProvider);
      return repository.getAll();
    });
  }

  Future<void> search(TrafficSignSearchPayload payload) async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final repository = ref.read(trafficSignRepositoryProvider);
      return repository.search(payload);
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

      // Listen cho notifications về contribution approval
      // Khi contribution được approve, nó sẽ tạo traffic sign mới
      connection.on('ReceiveNotification', (arguments) {
        if (arguments == null || arguments.isEmpty) return;
        final payload = arguments.first as Map<String, dynamic>;
        final notificationType = payload['type'] as String? ?? '';
        final message = payload['message'] as String? ?? '';
        
        // Nếu là notification về contribution approval, refresh signs
        if (notificationType.contains('Contribution') && 
            (message.contains('approved') || message.contains('được phê duyệt'))) {
          // Delay một chút để đảm bảo backend đã tạo sign
          Future.delayed(const Duration(seconds: 1), () {
            refreshSigns();
          });
        }
      });

      await connection.start();
      await connection.invoke('JoinUserGroup', args: [user.id]);
      _connection = connection;
    } catch (e) {
      // Nếu SignalR không available, chỉ dùng periodic refresh
      print('SignalR connection failed: $e');
    }
  }

  void _setupPeriodicRefresh() {
    // Refresh mỗi 30 giây để đảm bảo map luôn cập nhật
    _refreshTimer?.cancel();
    _refreshTimer = Timer.periodic(const Duration(seconds: 30), (timer) {
      if (state.hasValue) {
        refreshSigns();
      }
    });
  }

  Future<void> _disposeConnection() async {
    _refreshTimer?.cancel();
    if (_connection != null) {
      try {
        await _connection!.stop();
      } catch (e) {
        print('Error stopping SignalR connection: $e');
      }
      _connection = null;
    }
  }
}

final trafficSignControllerProvider =
    AutoDisposeAsyncNotifierProvider<TrafficSignController, List<TrafficSign>>(
      TrafficSignController.new,
    );
