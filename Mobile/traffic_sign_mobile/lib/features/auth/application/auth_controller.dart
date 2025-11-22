import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/firebase/firebase_service.dart';
import '../../../core/storage/token_storage.dart';
import '../data/auth_repository.dart';
import '../data/models/auth_user.dart';

enum AuthStatus { unknown, authenticated, unauthenticated }

class AuthState {
  const AuthState({
    this.status = AuthStatus.unknown,
    this.user,
    this.token,
    this.isLoading = false,
    this.errorMessage,
    this.isSessionChecked = false,
  });

  final AuthStatus status;
  final AuthUser? user;
  final String? token;
  final bool isLoading;
  final String? errorMessage;
  final bool isSessionChecked;

  bool get isAuthenticated => status == AuthStatus.authenticated;

  AuthState copyWith({
    AuthStatus? status,
    AuthUser? user,
    String? token,
    bool? isLoading,
    String? errorMessage,
    bool? isSessionChecked,
  }) {
    return AuthState(
      status: status ?? this.status,
      user: user ?? this.user,
      token: token ?? this.token,
      isLoading: isLoading ?? this.isLoading,
      errorMessage: errorMessage,
      isSessionChecked: isSessionChecked ?? this.isSessionChecked,
    );
  }
}

class AuthController extends StateNotifier<AuthState> {
  AuthController(this._ref) : super(const AuthState());

  final Ref _ref;

  Future<void> restoreSession() async {
    final storage = _ref.read(tokenStorageProvider);
    final user = storage.getStoredUser();
    final token = await storage.readAccessToken();

    if (user != null) {
      state = state.copyWith(
        status: AuthStatus.authenticated,
        user: user,
        token: token,
        isSessionChecked: true,
      );

      // Send FCM token to backend if user is already logged in
      _sendFCMTokenToBackend(user.id);
    } else {
      state = state.copyWith(
        status: AuthStatus.unauthenticated,
        isSessionChecked: true,
      );
    }
  }

  Future<void> login(String email, String password) async {
    state = state.copyWith(isLoading: true, errorMessage: null);
    try {
      final repository = _ref.read(authRepositoryProvider);
      final storage = _ref.read(tokenStorageProvider);
      final response = await repository.login(email: email, password: password);

      await storage.saveSession(token: response.token, user: response.user);
      state = state.copyWith(
        status: AuthStatus.authenticated,
        user: response.user,
        token: response.token,
        isLoading: false,
        errorMessage: null,
        isSessionChecked: true,
      );

      // Send FCM token to backend after successful login
      _sendFCMTokenToBackend(response.user.id);
    } catch (error) {
      state = state.copyWith(
        isLoading: false,
        errorMessage: error.toString(),
        status: AuthStatus.unauthenticated,
        isSessionChecked: true,
      );
    }
  }

  /// Send FCM token to backend (non-blocking)
  Future<void> _sendFCMTokenToBackend(int userId) async {
    try {
      final fcmToken = await FirebaseService.instance.getFCMToken();
      if (fcmToken != null) {
        final repository = _ref.read(authRepositoryProvider);
        await repository.saveFCMToken(userId: userId, fcmToken: fcmToken);
      }
    } catch (e) {
      // Silent fail - FCM token registration is not critical
    }
  }

  Future<void> register({
    required String username,
    required String email,
    required String password,
    required String phoneNumber,
  }) async {
    state = state.copyWith(isLoading: true, errorMessage: null);
    try {
      final repository = _ref.read(authRepositoryProvider);
      final user = await repository.register(
        username: username,
        email: email,
        password: password,
        phoneNumber: phoneNumber,
      );

      state = state.copyWith(isLoading: false, user: user);
    } catch (error) {
      state = state.copyWith(isLoading: false, errorMessage: error.toString());
    }
  }

  Future<void> logout() async {
    // Delete FCM token from backend before logout
    final userId = state.user?.id;
    if (userId != null) {
      try {
        final repository = _ref.read(authRepositoryProvider);
        await repository.deleteFCMToken(userId: userId);
      } catch (e) {
        // Silent fail - continue with logout
      }
    }

    // Delete FCM token locally
    try {
      await FirebaseService.instance.deleteToken();
    } catch (e) {
      // Silent fail - continue with logout
    }

    final storage = _ref.read(tokenStorageProvider);
    await storage.clear();
    state = const AuthState(
      status: AuthStatus.unauthenticated,
      isSessionChecked: true,
    );
  }

  Future<void> updateProfile({
    String? username,
    String? email,
    String? phoneNumber,
  }) async {
    if (state.user == null) return;
    
    state = state.copyWith(isLoading: true, errorMessage: null);
    try {
      final repository = _ref.read(authRepositoryProvider);
      final storage = _ref.read(tokenStorageProvider);
      final updatedUser = await repository.updateProfile(
        userId: state.user!.id,
        username: username,
        email: email,
        phoneNumber: phoneNumber,
      );

      await storage.saveSession(
        token: state.token,
        user: updatedUser,
      );
      
      state = state.copyWith(
        user: updatedUser,
        isLoading: false,
        errorMessage: null,
      );
    } catch (error) {
      state = state.copyWith(
        isLoading: false,
        errorMessage: error.toString(),
      );
    }
  }

  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    if (state.user == null) return;
    
    state = state.copyWith(isLoading: true, errorMessage: null);
    try {
      final repository = _ref.read(authRepositoryProvider);
      await repository.changePassword(
        userId: state.user!.id,
        currentPassword: currentPassword,
        newPassword: newPassword,
      );
      
      state = state.copyWith(
        isLoading: false,
        errorMessage: null,
      );
    } catch (error) {
      state = state.copyWith(
        isLoading: false,
        errorMessage: error.toString(),
      );
    }
  }

  Future<void> refreshProfile() async {
    if (state.user == null) return;
    
    try {
      final repository = _ref.read(authRepositoryProvider);
      final storage = _ref.read(tokenStorageProvider);
      final updatedUser = await repository.fetchProfile(state.user!.id);

      await storage.saveSession(
        token: state.token,
        user: updatedUser,
      );
      
      state = state.copyWith(user: updatedUser);
    } catch (error) {
      // Silent fail for refresh
    }
  }
}

final authControllerProvider = StateNotifierProvider<AuthController, AuthState>(
  (ref) {
    return AuthController(ref);
  },
);
