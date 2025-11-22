import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../features/auth/application/auth_controller.dart';
import '../features/auth/presentation/login_screen.dart';
import '../features/auth/presentation/register_screen.dart';
import '../features/auth/presentation/user_profile_screen.dart';
import '../features/contributions/presentation/contribution_form_screen.dart';
import '../features/home/presentation/home_shell.dart';
import '../features/map/presentation/map_search_screen.dart';
import '../features/splash/presentation/splash_screen.dart';

enum AppRoute {
  splash,
  login,
  register,
  home,
  contributionForm,
  mapSearch,
  userProfile,
}

final appRouterProvider = Provider<GoRouter>((ref) {
  final authState = ref.watch(authControllerProvider);

  return GoRouter(
    initialLocation: '/splash',
    debugLogDiagnostics: true,
    refreshListenable: GoRouterAuthListener(ref),
    routes: [
      GoRoute(
        path: '/splash',
        name: AppRoute.splash.name,
        builder: (context, state) => const SplashScreen(),
      ),
      GoRoute(
        path: '/auth/login',
        name: AppRoute.login.name,
        builder: (context, state) => const LoginScreen(),
      ),
      GoRoute(
        path: '/auth/register',
        name: AppRoute.register.name,
        builder: (context, state) => const RegisterScreen(),
      ),
      GoRoute(
        path: '/home',
        name: AppRoute.home.name,
        builder: (context, state) => const HomeShell(),
        routes: [
          GoRoute(
            path: 'contribution/new',
            name: AppRoute.contributionForm.name,
            builder: (context, state) => const ContributionFormScreen(),
          ),
          GoRoute(
            path: 'map/search',
            name: AppRoute.mapSearch.name,
            builder: (context, state) => const MapSearchScreen(),
          ),
          GoRoute(
            path: 'profile',
            name: AppRoute.userProfile.name,
            builder: (context, state) => const UserProfileScreen(),
          ),
        ],
      ),
    ],
    redirect: (context, state) {
      final isAuthed = authState.isAuthenticated;
      final isSessionChecked = authState.isSessionChecked;
      final loggingIn = state.matchedLocation.startsWith('/auth');

      if (!isSessionChecked) {
        return state.matchedLocation == '/splash' ? null : '/splash';
      }

      if (!isAuthed) {
        return loggingIn ? null : '/auth/login';
      }

      if (isAuthed && loggingIn) {
        return '/home';
      }

      if (state.matchedLocation == '/splash') {
        return '/home';
      }

      return null;
    },
  );
});

class GoRouterAuthListener extends ChangeNotifier {
  GoRouterAuthListener(this._ref) {
    _subscription = _ref.listen<AuthState>(
      authControllerProvider,
      (_, __) => notifyListeners(),
    );
  }

  final Ref _ref;
  late final ProviderSubscription<AuthState> _subscription;

  @override
  void dispose() {
    _subscription.close();
    super.dispose();
  }
}
