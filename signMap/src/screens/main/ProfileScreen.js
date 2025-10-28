import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  SafeAreaView,
  ScrollView,
  Alert,
  TextInput,
  ActivityIndicator,
} from 'react-native';
import { commonStyles, colors, typography, spacing, borderRadius, shadows } from '../../styles/theme';

const ProfileScreen = ({ navigation }) => {
  // Mock user data - in real app this would come from props or navigation params
  const mockUser = {
    full_name: 'Nguyễn Văn A',
    email: 'user@example.com',
    phone: '0123456789',
    reputation: 100,
    coin_balance: 50,
  };

  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [profileData, setProfileData] = useState({
    full_name: mockUser?.full_name || '',
    email: mockUser?.email || '',
    phone: mockUser?.phone || '',
  });

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleSave = async () => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      setIsEditing(false);
      Alert.alert('Thành công', 'Cập nhật thông tin thành công!');
    } catch (error) {
      Alert.alert('Lỗi', 'Có lỗi xảy ra, vui lòng thử lại');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    setProfileData({
      full_name: mockUser?.full_name || '',
      email: mockUser?.email || '',
      phone: mockUser?.phone || '',
    });
    setIsEditing(false);
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
      id: 'achievements',
      title: 'Thành tích',
      subtitle: 'Xem các thành tích đã đạt được',
      icon: '🏆',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'reports',
      title: 'Báo cáo của tôi',
      subtitle: 'Lịch sử báo cáo và đóng góp',
      icon: '📋',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'settings',
      title: 'Cài đặt',
      subtitle: 'Tùy chỉnh ứng dụng',
      icon: '⚙️',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'help',
      title: 'Trợ giúp',
      subtitle: 'Hướng dẫn sử dụng và hỗ trợ',
      icon: '❓',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
    {
      id: 'about',
      title: 'Về ứng dụng',
      subtitle: 'Thông tin phiên bản và nhà phát triển',
      icon: 'ℹ️',
      onPress: () => Alert.alert('Thông báo', 'Tính năng đang phát triển!'),
    },
  ];

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView style={styles.scrollView} showsVerticalScrollIndicator={false}>
        {/* Header */}
        <View style={styles.header}>
          <TouchableOpacity
            style={styles.backButton}
            onPress={() => navigation.goBack()}
          >
            <Text style={styles.backButtonText}>‹</Text>
          </TouchableOpacity>
          <Text style={styles.headerTitle}>Hồ sơ cá nhân</Text>
          <TouchableOpacity
            style={styles.headerButton}
            onPress={isEditing ? handleSave : handleEdit}
            disabled={isLoading}
          >
            {isLoading ? (
              <ActivityIndicator size="small" color={colors.primary} />
            ) : (
              <Text style={styles.headerButtonText}>
                {isEditing ? 'Lưu' : 'Sửa'}
              </Text>
            )}
          </TouchableOpacity>
        </View>

        {/* Profile Info */}
        <View style={styles.profileContainer}>
          <View style={styles.avatarContainer}>
            <Text style={styles.avatar}>👤</Text>
          </View>
          
          <View style={styles.profileInfo}>
            {isEditing ? (
              <View style={styles.editForm}>
                <View style={styles.inputContainer}>
                  <Text style={styles.label}>Họ và tên</Text>
                  <TextInput
                    style={styles.input}
                    value={profileData.full_name}
                    onChangeText={(text) => setProfileData(prev => ({ ...prev, full_name: text }))}
                    placeholder="Nhập họ và tên"
                  />
                </View>
                <View style={styles.inputContainer}>
                  <Text style={styles.label}>Email</Text>
                  <TextInput
                    style={[styles.input, styles.inputDisabled]}
                    value={profileData.email}
                    editable={false}
                    placeholder="Email không thể thay đổi"
                  />
                </View>
                <View style={styles.inputContainer}>
                  <Text style={styles.label}>Số điện thoại</Text>
                  <TextInput
                    style={styles.input}
                    value={profileData.phone}
                    onChangeText={(text) => setProfileData(prev => ({ ...prev, phone: text }))}
                    placeholder="Nhập số điện thoại"
                    keyboardType="phone-pad"
                  />
                </View>
                <View style={styles.editActions}>
                  <TouchableOpacity
                    style={styles.cancelButton}
                    onPress={handleCancel}
                  >
                    <Text style={styles.cancelButtonText}>Hủy</Text>
                  </TouchableOpacity>
                </View>
              </View>
            ) : (
              <View>
                <Text style={styles.userName}>{mockUser?.full_name || 'Chưa cập nhật'}</Text>
                <Text style={styles.userEmail}>{mockUser?.email}</Text>
                <Text style={styles.userPhone}>{mockUser?.phone || 'Chưa cập nhật'}</Text>
              </View>
            )}
          </View>
        </View>

        {/* Stats */}
        <View style={styles.statsContainer}>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>{mockUser?.reputation || 0}</Text>
            <Text style={styles.statLabel}>Điểm uy tín</Text>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>{mockUser?.coin_balance || 0}</Text>
            <Text style={styles.statLabel}>Xu</Text>
          </View>
          <View style={styles.statCard}>
            <Text style={styles.statNumber}>0</Text>
            <Text style={styles.statLabel}>Báo cáo</Text>
          </View>
        </View>

        {/* Menu Items */}
        <View style={styles.menuContainer}>
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

        {/* Logout Button */}
        <View style={styles.logoutContainer}>
          <TouchableOpacity
            style={styles.logoutButton}
            onPress={handleLogout}
          >
            <Text style={styles.logoutButtonText}>Đăng xuất</Text>
          </TouchableOpacity>
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
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: spacing.lg,
    backgroundColor: colors.primary,
  },
  backButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  backButtonText: {
    color: colors.white,
    fontSize: typography.fontSize.xl,
    fontWeight: typography.fontWeight.bold,
  },
  headerTitle: {
    fontSize: typography.fontSize.lg,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.white,
  },
  headerButton: {
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    borderRadius: borderRadius.md,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
  },
  headerButtonText: {
    color: colors.white,
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.medium,
  },
  profileContainer: {
    flexDirection: 'row',
    padding: spacing.lg,
    backgroundColor: colors.surface,
    ...shadows.sm,
  },
  avatarContainer: {
    width: 80,
    height: 80,
    borderRadius: 40,
    backgroundColor: colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: spacing.lg,
  },
  avatar: {
    fontSize: typography.fontSize['3xl'],
  },
  profileInfo: {
    flex: 1,
    justifyContent: 'center',
  },
  editForm: {
    flex: 1,
  },
  inputContainer: {
    marginBottom: spacing.md,
  },
  label: {
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.textPrimary,
    marginBottom: spacing.xs,
  },
  input: {
    ...commonStyles.input,
    height: 40,
    paddingHorizontal: spacing.sm,
  },
  inputDisabled: {
    backgroundColor: colors.gray100,
    color: colors.textTertiary,
  },
  editActions: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginTop: spacing.sm,
  },
  cancelButton: {
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    borderRadius: borderRadius.md,
    backgroundColor: colors.gray200,
  },
  cancelButtonText: {
    color: colors.textSecondary,
    fontSize: typography.fontSize.sm,
  },
  userName: {
    fontSize: typography.fontSize.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.textPrimary,
    marginBottom: spacing.xs,
  },
  userEmail: {
    fontSize: typography.fontSize.base,
    color: colors.textSecondary,
    marginBottom: spacing.xs,
  },
  userPhone: {
    fontSize: typography.fontSize.sm,
    color: colors.textTertiary,
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
  logoutContainer: {
    padding: spacing.lg,
  },
  logoutButton: {
    backgroundColor: colors.error,
    padding: spacing.lg,
    borderRadius: borderRadius.lg,
    alignItems: 'center',
    ...shadows.sm,
  },
  logoutButtonText: {
    color: colors.white,
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.semiBold,
  },
});

export default ProfileScreen;
