import 'package:flutter/material.dart';

import '../../contributions/presentation/user_contributions_screen.dart';
import '../../map/presentation/map_screen.dart';
import '../../notifications/presentation/notifications_screen.dart';
import '../../votes/presentation/voting_screen.dart';
import '../../wallet/presentation/wallet_screen.dart';

class HomeShell extends StatefulWidget {
  const HomeShell({super.key});

  @override
  State<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends State<HomeShell> {
  int _index = 0;

  final _tabs = const [
    MapScreen(),
    UserContributionsScreen(),
    VotingScreen(),
    WalletScreen(),
    NotificationsScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
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
