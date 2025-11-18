import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../data/models/traffic_sign.dart';
import '../data/traffic_sign_repository.dart';
import '../domain/traffic_sign_search_payload.dart';

class TrafficSignController
    extends AutoDisposeAsyncNotifier<List<TrafficSign>> {
  @override
  Future<List<TrafficSign>> build() async {
    final repository = ref.read(trafficSignRepositoryProvider);
    return repository.getAll();
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
}

final trafficSignControllerProvider =
    AutoDisposeAsyncNotifierProvider<TrafficSignController, List<TrafficSign>>(
      TrafficSignController.new,
    );
