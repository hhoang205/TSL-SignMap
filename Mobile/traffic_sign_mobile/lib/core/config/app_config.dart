import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class AppConfig {
  const AppConfig({required this.apiBaseUrl, required this.notificationHubUrl});

  final String apiBaseUrl;
  final String notificationHubUrl;

  static AppConfig fromEnv() {
    final apiBaseUrl = dotenv.env['API_BASE_URL'] ?? 'http://10.0.2.2:5000';
    final notificationHubUrl =
        dotenv.env['NOTIFICATION_HUB_URL'] ??
        'http://10.0.2.2:5207/hubs/notifications';

    return AppConfig(
      apiBaseUrl: apiBaseUrl,
      notificationHubUrl: notificationHubUrl,
    );
  }
}

final appConfigProvider = Provider<AppConfig>((ref) {
  return AppConfig.fromEnv();
});
