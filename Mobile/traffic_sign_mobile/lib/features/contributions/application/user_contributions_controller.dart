import 'dart:io';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../auth/application/auth_controller.dart';
import '../../wallet/application/wallet_controller.dart';
import '../../wallet/data/wallet_balance.dart';
import '../data/contribution_repository.dart';
import '../data/image_upload_repository.dart';
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

  Future<String?> checkCoinBalance() async {
    try {
      // Refresh wallet to get latest balance before checking
      await ref.read(walletControllerProvider.notifier).refresh();
      
      // Give a small delay to ensure state is updated after refresh
      await Future.delayed(const Duration(milliseconds: 100));
      
      // Read the wallet state
      final walletAsync = ref.read(walletControllerProvider);
      
      // Wait for state to be ready if still loading
      if (walletAsync.isLoading) {
        // Wait up to 2 seconds for loading to complete
        int attempts = 0;
        const maxAttempts = 20;
        while (attempts < maxAttempts) {
          await Future.delayed(const Duration(milliseconds: 100));
          final currentState = ref.read(walletControllerProvider);
          if (!currentState.isLoading) {
            break;
          }
          attempts++;
        }
      }
      
      // Get the final state
      final finalState = ref.read(walletControllerProvider);
      
      // Check for errors
      if (finalState.hasError) {
        return 'Không thể kiểm tra số dư. Vui lòng thử lại.';
      }
      
      // Get wallet balance
      final walletState = finalState.valueOrNull;
      if (walletState == null) {
        return 'Không thể kiểm tra số dư. Vui lòng thử lại.';
      }
      
      // Check if balance is sufficient (5 coins required)
      if (walletState.balance < 5.0) {
        return 'Không đủ coin để submit contribution. Cần 5 coin. Số dư hiện tại: ${walletState.balance.toStringAsFixed(1)} coin.';
      }
      
      return null;
    } catch (e) {
      // If wallet check fails, return error message
      return 'Không thể kiểm tra số dư. Vui lòng thử lại.';
    }
  }

  Future<String?> uploadImage(File imageFile) async {
    try {
      final uploadRepo = ref.read(imageUploadRepositoryProvider);
      final url = await uploadRepo.uploadImage(imageFile);
      // Return null if upload failed (empty string means endpoint not available)
      return url.isEmpty ? null : url;
    } catch (e) {
      // Return null on error - contribution will be submitted without image
      return null;
    }
  }

  Future<void> submitContribution({
    required String action,
    required String? type,
    required String? description,
    required double? latitude,
    required double? longitude,
    String? imagePath,
    File? imageFile,
    int? signId,
  }) async {
    final user = ref.read(authControllerProvider).user;
    if (user == null) return;

    // Check coin balance (5 coins required)
    final balanceError = await checkCoinBalance();
    if (balanceError != null) {
      throw Exception(balanceError);
    }

    // Upload image if provided
    String? imageUrl = imagePath;
    if (imageFile != null) {
      final uploadedUrl = await uploadImage(imageFile);
      imageUrl = uploadedUrl ?? imagePath;
    }

    // Build payload - only include non-null values for optional fields
    final payload = <String, dynamic>{
      'userId': user.id,
      'action': action,
    };
    
    // For "Add" action, type, latitude, and longitude are required
    if (action == 'Add') {
      // Validate required fields for Add action
      if (type == null || type.isEmpty) {
        throw Exception('Type is required for Add action');
      }
      if (latitude == null || longitude == null) {
        throw Exception('Latitude and Longitude are required for Add action');
      }
      payload['type'] = type;
      payload['latitude'] = latitude;
      payload['longitude'] = longitude;
    }
    
    // Optional fields
    if (description != null && description.isNotEmpty) {
      payload['description'] = description;
    }
    if (imageUrl != null && imageUrl.isNotEmpty) {
      payload['imageUrl'] = imageUrl;
    }
    if (signId != null) {
      payload['signId'] = signId;
    }
    
    // For "Update" or "Delete" actions, include type if provided
    if ((action == 'Update' || action == 'Delete') && type != null && type.isNotEmpty) {
      payload['type'] = type;
    }

    await ref.read(contributionRepositoryProvider).submitContribution(payload);
    await refresh();
    
    // Refresh wallet to show updated balance
    ref.read(walletControllerProvider.notifier).refresh();
  }
}

final userContributionsControllerProvider =
    AutoDisposeAsyncNotifierProvider<
      UserContributionsController,
      List<Contribution>
    >(UserContributionsController.new);

