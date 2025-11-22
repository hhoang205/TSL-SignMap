import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/network/api_client.dart';
import 'models/auth_response.dart';
import 'models/auth_user.dart';

class AuthRepository {
  AuthRepository(this._apiClient);

  final ApiClient _apiClient;
  static const _basePath = '/api/users';

  Future<AuthResponse> login({
    required String email,
    required String password,
  }) async {
    final response = await _apiClient.post<Map<String, dynamic>>(
      '$_basePath/login',
      data: {'email': email, 'password': password},
    );
    return AuthResponse.fromJson(response.data!);
  }

  Future<AuthUser> register({
    required String username,
    required String email,
    required String password,
    required String phoneNumber,
  }) async {
    final response = await _apiClient.post<Map<String, dynamic>>(
      '$_basePath/register',
      data: {
        'username': username,
        'email': email,
        'password': password,
        'phoneNumber': phoneNumber,
      },
    );

    final data = response.data?['user'] as Map<String, dynamic>? ?? {};
    return AuthUser.fromJson(data);
  }

  Future<AuthUser> fetchProfile(int userId) async {
    final response = await _apiClient.get<Map<String, dynamic>>(
      '$_basePath/$userId',
    );
    return AuthUser.fromJson(response.data!);
  }

  Future<AuthUser> updateProfile({
    required int userId,
    String? username,
    String? email,
    String? phoneNumber,
  }) async {
    final response = await _apiClient.put<Map<String, dynamic>>(
      '$_basePath/$userId',
      data: {
        if (username != null) 'username': username,
        if (email != null) 'email': email,
        if (phoneNumber != null) 'phoneNumber': phoneNumber,
      },
    );
    final data = response.data?['user'] as Map<String, dynamic>? ?? response.data!;
    return AuthUser.fromJson(data);
  }

  Future<void> changePassword({
    required int userId,
    required String currentPassword,
    required String newPassword,
  }) async {
    await _apiClient.post<Map<String, dynamic>>(
      '$_basePath/$userId/change-password',
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
      },
    );
  }

  /// Save FCM token to backend
  Future<void> saveFCMToken({
    required int userId,
    required String fcmToken,
  }) async {
    await _apiClient.post<Map<String, dynamic>>(
      '$_basePath/$userId/fcm-token',
      data: {'fcmToken': fcmToken},
    );
  }

  /// Delete FCM token from backend
  Future<void> deleteFCMToken({
    required int userId,
  }) async {
    await _apiClient.delete<Map<String, dynamic>>(
      '$_basePath/$userId/fcm-token',
    );
  }
}

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return AuthRepository(apiClient);
});
