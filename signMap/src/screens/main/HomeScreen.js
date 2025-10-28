import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  SafeAreaView,
  ScrollView,
  Alert,
} from 'react-native';
import { commonStyles, colors, typography, spacing, borderRadius, shadows } from '../../styles/theme';

const HomeScreen = ({ navigation }) => {
  // Mock user data
  const user = {
    full_name: 'Nguyễn Văn A',
    reputation: 100,
    coin_balance: 50,
  };

  const handleLogout = () => {
    Alert.alert(
      'Đăng xuất',
      'Bạn có chắc chắn muốn đăng xuất?',
      [
        { text: 'Hủy', style: 'cancel' },
        { text: 'Đăng xuất', style: 'destructive', onPress: () => {
          // Navigate back to LoginScreen
          navigation.navigate('Login');
        }},
      ]
    );
  };

  const menuItems = [
    {
      id: 'map',
      title: 'Bản đồ biển báo',
      subtitle: 'Xem vị trí các biển báo giao thông',
      icon: '🗺️',
      onPress: () => navigation.navigate('Map'),
    },
    {
      id: 'camera',
      title: 'Chụp ảnh biển báo',
      subtitle: 'Báo cáo biển báo mới hoặc sự cố',
      icon: '📸',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'reports',
      title: 'Báo cáo của tôi',
      subtitle: 'Xem lịch sử báo cáo',
      icon: '📋',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'profile',
      title: 'Hồ sơ cá nhân',
      subtitle: 'Quản lý thông tin tài khoản',
      icon: '👤',
      onPress: () => navigation.navigate('Profile'),
    },
    {
      id: 'leaderboard',
      title: 'Bảng xếp hạng',
      subtitle: 'Xem thứ hạng cộng đồng',
      icon: '🏆',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'settings',
      title: 'Cài đặt',
      subtitle: 'Tùy chỉnh ứng dụng',
      icon: '⚙️',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
  ];

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView style={styles.scrollView} showsVerticalScrollIndicator={false}>
        {/* Header */}
        <View style={styles.header}>
          <View>
            <Text style={styles.greeting}>Xin chào!</Text>
            <Text style={styles.userName}>{user?.full_name || 'Người dùng'}</Text>
          </View>
          <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
            <Text style={styles.logoutText}>Đăng xuất</Text>
          </TouchableOpacity>
        </View>

        {/* Stats Cards */}
        <View style={styles.statsContainer}>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>{user?.reputation || 0}</Text>
            <Text style={styles.statLabel}>Điểm uy tín</Text>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>{user?.coin_balance || 0}</Text>
            <Text style={styles.statLabel}>Xu</Text>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>0</Text>
            <Text style={styles.statLabel}>Báo cáo</Text>
          </View>
        </View>

        {/* Menu Items */}
        <View style={styles.menuContainer}>
          <Text style={styles.sectionTitle}>Chức năng chính</Text>
          {menuItems.map((item) => (
            <TouchableOpacity
              key={item.id}
              style={styles.menuItem}
              onPress={item.onPress}
            >
              <View style={styles.menuItemLeft}>
                <Text style={styles.menuItemIcon}>{item.icon}</Text>
                <View style={styles.menuItemText}>
                  <Text style={styles.menuItemTitle}>{item.title}</Text>
                  <Text style={styles.menuItemSubtitle}>{item.subtitle}</Text>
                </View>
              </View>
              <Text style={styles.menuItemArrow}>›</Text>
            </TouchableOpacity>
          ))}
        </View>

        {/* Quick Actions */}
        <View style={styles.quickActionsContainer}>
          <Text style={styles.sectionTitle}>Thao tác nhanh</Text>
          <View style={styles.quickActions}>
            <TouchableOpacity
              style={styles.quickActionButton}
              onPress={() => navigation.navigate('Map')}
            >
              <Text style={styles.quickActionIcon}>🗺️</Text>
              <Text style={styles.quickActionText}>Xem bản đồ</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.quickActionButton}
              onPress={() => Alert.alert('Thông báo', 'Tính năng đang phát triển!')}
            >
              <Text style={styles.quickActionIcon}>📸</Text>
              <Text style={styles.quickActionText}>Chụp ảnh</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.quickActionButton}
              onPress={() => Alert.alert('Thông báo', 'Tính năng đang phát triển!')}
            >
              <Text style={styles.quickActionIcon}>📊</Text>
              <Text style={styles.quickActionText}>Thống kê</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    ...commonStyles.container,
  },
  scrollView: {
    flex: 1,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: spacing.lg,
    backgroundColor: colors.primary,
  },
  greeting: {
    fontSize: typography.fontSize.lg,
    color: colors.white,
    opacity: 0.9,
  },
  userName: {
    fontSize: typography.fontSize['2xl'],
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
  },
  logoutButton: {
    padding: spacing.sm,
  },
  logoutText: {
    color: colors.white,
    fontSize: typography.fontSize.sm,
    opacity: 0.9,
  },
  statsContainer: {
    flexDirection: 'row',
    padding: spacing.lg,
    gap: spacing.md,
  },
  statCard: {
    flex: 1,
    backgroundColor: colors.surface,
    padding: spacing.lg,
    borderRadius: borderRadius.lg,
    alignItems: 'center',
    ...shadows.sm,
  },
  statNumber: {
    fontSize: typography.fontSize['2xl'],
    fontWeight: typography.fontWeight.bold,
    color: colors.primary,
    marginBottom: spacing.xs,
  },
  statLabel: {
    fontSize: typography.fontSize.sm,
    color: colors.textSecondary,
  },
  menuContainer: {
    padding: spacing.lg,
  },
  sectionTitle: {
    fontSize: typography.fontSize.lg,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.textPrimary,
    marginBottom: spacing.md,
  },
  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: colors.surface,
    padding: spacing.lg,
    borderRadius: borderRadius.lg,
    marginBottom: spacing.sm,
    ...shadows.sm,
  },
  menuItemLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  menuItemIcon: {
    fontSize: typography.fontSize['2xl'],
    marginRight: spacing.md,
  },
  menuItemText: {
    flex: 1,
  },
  menuItemTitle: {
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.textPrimary,
    marginBottom: spacing.xs,
  },
  menuItemSubtitle: {
    fontSize: typography.fontSize.sm,
    color: colors.textSecondary,
  },
  menuItemArrow: {
    fontSize: typography.fontSize.xl,
    color: colors.textTertiary,
  },
  quickActionsContainer: {
    padding: spacing.lg,
    paddingTop: 0,
  },
  quickActions: {
    flexDirection: 'row',
    gap: spacing.md,
  },
  quickActionButton: {
    flex: 1,
    backgroundColor: colors.surface,
    padding: spacing.lg,
    borderRadius: borderRadius.lg,
    alignItems: 'center',
    ...shadows.sm,
  },
  quickActionIcon: {
    fontSize: typography.fontSize['2xl'],
    marginBottom: spacing.sm,
  },
  quickActionText: {
    fontSize: typography.fontSize.sm,
    color: colors.textPrimary,
    textAlign: 'center',
  },
});

export default HomeScreen;
