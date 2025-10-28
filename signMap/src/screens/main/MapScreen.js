import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  SafeAreaView,
  Alert,
  Dimensions,
  ScrollView,
} from 'react-native';
import { commonStyles, colors, typography, spacing, borderRadius, shadows } from '../../styles/theme';

const { width, height } = Dimensions.get('window');

const MapScreen = ({ navigation }) => {
  const [selectedFilter, setSelectedFilter] = useState('all');

  const filterOptions = [
    { id: 'all', label: 'Tất cả', icon: '🚦' },
    { id: 'speed', label: 'Tốc độ', icon: '⚡' },
    { id: 'stop', label: 'Dừng', icon: '🛑' },
    { id: 'warning', label: 'Cảnh báo', icon: '⚠️' },
    { id: 'info', label: 'Thông tin', icon: 'ℹ️' },
  ];

  const handleFilterPress = (filterId) => {
    setSelectedFilter(filterId);
    Alert.alert('Thông báo', `Đã chọn bộ lọc: ${filterOptions.find(f => f.id === filterId)?.label}`);
  };

  const handleLocationPress = () => {
    Alert.alert('Thông báo', 'Tính năng GPS đang phát triển!');
  };

  const handleSearchPress = () => {
    Alert.alert('Thông báo', 'Tính năng tìm kiếm đang phát triển!');
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity
          style={styles.backButton}
          onPress={() => navigation.goBack()}
        >
          <Text style={styles.backButtonText}>‹</Text>
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Bản đồ biển báo</Text>
        <TouchableOpacity
          style={styles.searchButton}
          onPress={handleSearchPress}
        >
          <Text style={styles.searchButtonText}>🔍</Text>
        </TouchableOpacity>
      </View>

      {/* Map Placeholder */}
      <View style={styles.mapContainer}>
        <View style={styles.mapPlaceholder}>
          <Text style={styles.mapIcon}>🗺️</Text>
          <Text style={styles.mapTitle}>Bản đồ biển báo</Text>
          <Text style={styles.mapSubtitle}>
            Hiển thị vị trí các biển báo giao thông
          </Text>
          <Text style={styles.mapDescription}>
            Tích hợp OpenStreetMap hoặc Google Maps
          </Text>
        </View>

        {/* Map Controls */}
        <View style={styles.mapControls}>
          <TouchableOpacity
            style={styles.controlButton}
            onPress={handleLocationPress}
          >
            <Text style={styles.controlButtonText}>📍</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.controlButton}
            onPress={() => Alert.alert('Thông báo', 'Tính năng phóng to đang phát triển!')}
          >
            <Text style={styles.controlButtonText}>+</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.controlButton}
            onPress={() => Alert.alert('Thông báo', 'Tính năng thu nhỏ đang phát triển!')}
          >
            <Text style={styles.controlButtonText}>-</Text>
          </TouchableOpacity>
        </View>
      </View>

      {/* Filter Bar */}
      <View style={styles.filterContainer}>
        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={styles.filterScrollContent}
        >
          {filterOptions.map((option) => (
            <TouchableOpacity
              key={option.id}
              style={[
                styles.filterButton,
                selectedFilter === option.id && styles.filterButtonActive,
              ]}
              onPress={() => handleFilterPress(option.id)}
            >
              <Text style={styles.filterIcon}>{option.icon}</Text>
              <Text
                style={[
                  styles.filterText,
                  selectedFilter === option.id && styles.filterTextActive,
                ]}
              >
                {option.label}
              </Text>
            </TouchableOpacity>
          ))}
        </ScrollView>
      </View>

      {/* Bottom Info Panel */}
      <View style={styles.infoPanel}>
        <View style={styles.infoHeader}>
          <Text style={styles.infoTitle}>Thông tin khu vực</Text>
          <TouchableOpacity
            style={styles.infoButton}
            onPress={() => Alert.alert('Thông báo', 'Tính năng đang phát triển!')}
          >
            <Text style={styles.infoButtonText}>Chi tiết</Text>
          </TouchableOpacity>
        </View>
        <Text style={styles.infoText}>
          Tìm thấy 0 biển báo trong khu vực này
        </Text>
        <Text style={styles.infoSubtext}>
          Di chuyển bản đồ để xem thêm biển báo
        </Text>
      </View>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    ...commonStyles.container,
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
  searchButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  searchButtonText: {
    fontSize: typography.fontSize.lg,
  },
  mapContainer: {
    flex: 1,
    position: 'relative',
  },
  mapPlaceholder: {
    flex: 1,
    backgroundColor: colors.gray100,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 2,
    borderColor: colors.border,
    borderStyle: 'dashed',
    margin: spacing.lg,
    borderRadius: borderRadius.lg,
  },
  mapIcon: {
    fontSize: 60,
    marginBottom: spacing.md,
  },
  mapTitle: {
    fontSize: typography.fontSize['2xl'],
    fontWeight: typography.fontWeight.bold,
    color: colors.textPrimary,
    marginBottom: spacing.sm,
  },
  mapSubtitle: {
    fontSize: typography.fontSize.lg,
    color: colors.textSecondary,
    textAlign: 'center',
    marginBottom: spacing.sm,
  },
  mapDescription: {
    fontSize: typography.fontSize.sm,
    color: colors.textTertiary,
    textAlign: 'center',
  },
  mapControls: {
    position: 'absolute',
    right: spacing.lg,
    top: spacing.lg,
    gap: spacing.sm,
  },
  controlButton: {
    width: 50,
    height: 50,
    borderRadius: 25,
    backgroundColor: colors.white,
    justifyContent: 'center',
    alignItems: 'center',
    ...shadows.md,
  },
  controlButtonText: {
    fontSize: typography.fontSize.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.textPrimary,
  },
  filterContainer: {
    backgroundColor: colors.surface,
    paddingVertical: spacing.md,
    ...shadows.sm,
  },
  filterScrollContent: {
    paddingHorizontal: spacing.lg,
    gap: spacing.sm,
  },
  filterButton: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    borderRadius: borderRadius.full,
    backgroundColor: colors.gray100,
    marginRight: spacing.sm,
  },
  filterButtonActive: {
    backgroundColor: colors.primary,
  },
  filterIcon: {
    fontSize: typography.fontSize.base,
    marginRight: spacing.xs,
  },
  filterText: {
    fontSize: typography.fontSize.sm,
    color: colors.textSecondary,
    fontWeight: typography.fontWeight.medium,
  },
  filterTextActive: {
    color: colors.white,
  },
  infoPanel: {
    backgroundColor: colors.surface,
    padding: spacing.lg,
    ...shadows.md,
  },
  infoHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.sm,
  },
  infoTitle: {
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.textPrimary,
  },
  infoButton: {
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.md,
    backgroundColor: colors.primary,
  },
  infoButtonText: {
    fontSize: typography.fontSize.sm,
    color: colors.white,
    fontWeight: typography.fontWeight.medium,
  },
  infoText: {
    fontSize: typography.fontSize.sm,
    color: colors.textSecondary,
    marginBottom: spacing.xs,
  },
  infoSubtext: {
    fontSize: typography.fontSize.xs,
    color: colors.textTertiary,
  },
});

export default MapScreen;
