import React, { useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ActivityIndicator,
} from 'react-native';
import { commonStyles, colors, typography, spacing } from '../styles/theme';

const SplashScreen = () => {
  return (
    <View style={styles.container}>
      <View style={styles.content}>
        <Text style={styles.emoji}>🚦</Text>
        <Text style={styles.title}>SignMap</Text>
        <Text style={styles.subtitle}>Quản lý biển báo giao thông thông minh</Text>
        
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
          <Text style={styles.loadingText}>Đang tải...</Text>
        </View>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.primary,
  },
  content: {
    ...commonStyles.centerContent,
    padding: spacing.lg,
  },
  emoji: {
    fontSize: 80,
    marginBottom: spacing.lg,
  },
  title: {
    fontSize: typography.fontSize['5xl'],
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
    marginBottom: spacing.md,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: typography.fontSize.lg,
    color: colors.white,
    opacity: 0.9,
    textAlign: 'center',
    marginBottom: spacing['3xl'],
  },
  loadingContainer: {
    alignItems: 'center',
  },
  loadingText: {
    color: colors.white,
    fontSize: typography.fontSize.base,
    marginTop: spacing.md,
    opacity: 0.8,
  },
});

export default SplashScreen;
