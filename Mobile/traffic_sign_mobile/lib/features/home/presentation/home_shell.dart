import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

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

  final List<String> _tabTitles = const [
    'Bản đồ',
    'Đóng góp',
    'Bỏ phiếu',
    'Ví',
    'Thông báo',
  ];

  List<Widget> _getAppBarActions(BuildContext context) {
    switch (_index) {
      case 0: // Map
        return [
          IconButton(
            icon: const Icon(Icons.filter_alt),
            tooltip: 'Tìm kiếm nâng cao',
            onPressed: () => context.push('/home/map/search'),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: 'Tài khoản',
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 8),
                    Text('Hồ sơ'),
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
            tooltip: 'Tạo đóng góp mới',
            onPressed: () => context.push('/home/contribution/new'),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: 'Tài khoản',
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 8),
                    Text('Hồ sơ'),
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
            tooltip: 'Làm mới',
            onPressed: () =>
                ref.read(walletControllerProvider.notifier).refresh(),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: 'Tài khoản',
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 8),
                    Text('Hồ sơ'),
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
            tooltip: 'Làm mới',
            onPressed: () =>
                ref.read(notificationControllerProvider.notifier).refresh(),
          ),
          PopupMenuButton<String>(
            icon: const Icon(Icons.account_circle_outlined),
            tooltip: 'Tài khoản',
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 8),
                    Text('Hồ sơ'),
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
            tooltip: 'Tài khoản',
            onSelected: (value) {
              if (value == 'profile') {
                context.push('/home/profile');
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person_outline, size: 20),
                    SizedBox(width: 8),
                    Text('Hồ sơ'),
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
    return Scaffold(
      appBar: AppBar(
        title: Text(_tabTitles[_index]),
        actions: _getAppBarActions(context),
      ),
      body: _tabs[_index],
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (value) => setState(() => _index = value),
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.map_outlined),
            label: 'Bản đồ',
          ),
          NavigationDestination(
            icon: Icon(Icons.add_location_alt_outlined),
            label: 'Đóng góp',
          ),
          NavigationDestination(
            icon: Icon(Icons.how_to_vote),
            label: 'Bỏ phiếu',
          ),
          NavigationDestination(
            icon: Icon(Icons.account_balance_wallet_outlined),
            label: 'Ví',
          ),
          NavigationDestination(
            icon: Icon(Icons.notifications_outlined),
            label: 'Thông báo',
          ),
        ],
      ),
    );
  }
}
