import 'package:flutter/material.dart';

/// Helper class để map traffic sign types thành màu sắc và icons
class MapMarkerHelper {
  /// Lấy màu sắc cho marker dựa trên sign type
  static Color getMarkerColor(String signType) {
    final type = signType.toLowerCase();
    
    if (type.contains('stop') || type.contains('dừng')) {
      return Colors.red;
    } else if (type.contains('speed') || type.contains('tốc độ')) {
      return Colors.orange;
    } else if (type.contains('warning') || type.contains('cảnh báo')) {
      return Colors.yellow.shade700;
    } else if (type.contains('prohibition') || type.contains('cấm')) {
      return Colors.red.shade800;
    } else if (type.contains('mandatory') || type.contains('bắt buộc')) {
      return Colors.blue;
    } else if (type.contains('information') || type.contains('thông tin')) {
      return Colors.green;
    } else if (type.contains('direction') || type.contains('chỉ dẫn')) {
      return Colors.blue.shade700;
    } else {
      return Colors.grey.shade700;
    }
  }

  /// Lấy icon cho marker dựa trên sign type
  static IconData getMarkerIcon(String signType) {
    final type = signType.toLowerCase();
    
    if (type.contains('stop') || type.contains('dừng')) {
      return Icons.stop_circle;
    } else if (type.contains('speed') || type.contains('tốc độ')) {
      return Icons.speed;
    } else if (type.contains('warning') || type.contains('cảnh báo')) {
      return Icons.warning;
    } else if (type.contains('prohibition') || type.contains('cấm')) {
      return Icons.block;
    } else if (type.contains('mandatory') || type.contains('bắt buộc')) {
      return Icons.arrow_forward;
    } else if (type.contains('information') || type.contains('thông tin')) {
      return Icons.info;
    } else if (type.contains('direction') || type.contains('chỉ dẫn')) {
      return Icons.navigation;
    } else {
      return Icons.location_on;
    }
  }

  /// Lấy tên hiển thị cho sign type
  static String getDisplayName(String signType) {
    // Có thể map từ English sang Vietnamese nếu cần
    return signType;
  }
}

