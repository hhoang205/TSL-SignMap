import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../l10n/app_localizations.dart';
import '../../contributions/presentation/user_contributions_screen.dart';
import '../../map/presentation/map_screen.dart';
import '../../notifications/presentation/notifications_screen.dart';
import '../../notifications/application/notification_controller.dart';
import '../../votes/presentation/voting_screen.dart';
import '../../wallet/presentation/wallet_screen.dart';
import '../../wallet/application/wallet_controller.dart';

class HomeShell extends ConsumerStatefulWidget {
  const HomeShell({super.key});

  @override
  ConsumerState<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends ConsumerState<HomeShell> {
  int _index = 0;

  final _tabs = const [
    MapScreen(),
    UserContributionsScreen(),
    VotingScreen(),
    WalletScreen(),
    NotificationsScreen(),
  ];

  List<String> _getTabTitles(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return [
      l10n.map,
      l10n.contributions,
      l10n.voting,
      l10n.wallet,
      l10n.notifications,
    ];
  }

  List<Widget> _getAppBarActions(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    switch (_index) {
      case 0: // Map
        return [
          IconButton(
            icon: const Icon(Icons.filter_alt),
            tooltip: l10n.advancedSearch,
            onPressed: () => context.push('/home/map/search'),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: l10n.account,
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    const Icon(Icons.person_outline, size: 20),
                    const SizedBox(width: 8),
                    Text(l10n.profile),
                  ],
                ),
              ),
            ],
          ),
        ];
      case 1: // Contributions
        return [
          IconButton(
            icon: const Icon(Icons.add),
            tooltip: l10n.createNewContribution,
            onPressed: () => context.push('/home/contribution/new'),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: l10n.account,
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    const Icon(Icons.person_outline, size: 20),
                    const SizedBox(width: 8),
                    Text(l10n.profile),
                  ],
                ),
              ),
            ],
          ),
        ];
      case 3: // Wallet
        return [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: l10n.refresh,
            onPressed: () =>
                ref.read(walletControllerProvider.notifier).refresh(),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: l10n.account,
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    const Icon(Icons.person_outline, size: 20),
                    const SizedBox(width: 8),
                    Text(l10n.profile),
                  ],
                ),
              ),
            ],
          ),
        ];
      case 4: // Notifications
        return [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: l10n.refresh,
            onPressed: () =>
                ref.read(notificationControllerProvider.notifier).refresh(),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: l10n.account,
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    const Icon(Icons.person_outline, size: 20),
                    const SizedBox(width: 8),
                    Text(l10n.profile),
                  ],
                ),
              ),
            ],
          ),
        ];
      default: // Voting and others
        return [
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: l10n.account,
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    const Icon(Icons.person_outline, size: 20),
                    const SizedBox(width: 8),
                    Text(l10n.profile),
                  ],
                ),
              ),
            ],
          ),
        ];
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final tabTitles = _getTabTitles(context);
    
    return Scaffold(
      appBar: AppBar(
        title: Text(tabTitles[_index]),
        actions: _getAppBarActions(context),
      ),
      body: _tabs[_index],
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (value) => setState(() => _index = value),
        destinations: [
          NavigationDestination(
            icon: const Icon(Icons.map_outlined),
            label: l10n.map,
          ),
          NavigationDestination(
            icon: const Icon(Icons.add_location_alt_outlined),
            label: l10n.contributions,
          ),
          NavigationDestination(
            icon: const Icon(Icons.how_to_vote),
            label: l10n.voting,
          ),
          NavigationDestination(
            icon: const Icon(Icons.account_balance_wallet_outlined),
            label: l10n.wallet,
          ),
          NavigationDestination(
            icon: const Icon(Icons.notifications_outlined),
            label: l10n.notifications,
          ),
        ],
      ),
    );
  }
}
