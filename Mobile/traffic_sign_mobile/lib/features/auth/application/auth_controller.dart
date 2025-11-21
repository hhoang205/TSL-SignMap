import 'package:flutter_riverpod/flutter_riverpod.dart';
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
    } catch (error) {
      state = state.copyWith(
        isLoading: false,
        errorMessage: error.toString(),
        status: AuthStatus.unauthenticated,
        isSessionChecked: true,
      );
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

  Future<void> fetchUserProfile(int userId) async {
    state = state.copyWith(isLoading: true);

    try {
      final repository = _ref.read(authRepositoryProvider);
      final user = await repository.fetchProfile(userId);

      state = state.copyWith(
        user: user,
        isLoading: false,
        status: AuthStatus.authenticated,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        errorMessage: e.toString(),
      );
    }
  }

  Future<void> logout() async {
    final storage = _ref.read(tokenStorageProvider);
    await storage.clear();

    state = const AuthState(
      status: AuthStatus.unauthenticated,
      isSessionChecked: true,
    );
  }
}

final authControllerProvider =
    StateNotifierProvider<AuthController, AuthState>(
  (ref) => AuthController(ref),
);
