# Traffic Sign Mobile App (Flutter)

Flutter client for the WebAppTrafficSign platform. Implements the user-facing roadmap in `todo.md` section 9.

## Features (initial milestone)

- Firebase-ready auth flow calling `UserService` login/register APIs (email + password) with secure token storage.
- Bottom navigation shell with tabs for Map, Contributions, Voting, Wallet, and Notifications.
- Map tab: OpenStreetMap layer via `flutter_map`, displays live traffic signs and advanced filter screen (radius/type coin charge hooks).
- Contribution tab: list personal submissions, refresh, and submit new entries with camera/photo capture + GPS metadata.
- Voting tab: lists pending contributions and lets users cast weighted up/down votes.
- Wallet tab: shows real-time coin balance (future payment gateway hook placeholder).
- Notifications tab: fetches NotificationService history and subscribes to SignalR hub for real-time updates.

## Project setup (Task 9.1)

```bash
cd Mobile/traffic_sign_mobile
flutter pub get
copy env.sample .env   # adjust base URLs as needed
```

`env.sample` contains default localhost URLs (Android emulator friendly). `flutter_dotenv` loads `.env` at runtime.

## Running

```bash
flutter run -d chrome        # or android, ios, windows
```

The app assumes the API Gateway exposes the backend services at `API_BASE_URL`. When running in Android emulator, `10.0.2.2` points to the host machine; replace with LAN IP or production URL as required.

## Next steps

- Integrate real image upload & AI detection service before submission.
- Implement coin charging feedback in UI after advanced search/submit/vote actions.
- Wire payment top-up screen to PaymentService once gateway integration is ready.
- Expand notification center with mark-read/delete actions and push notifications.
# traffic_sign_mobile

A new Flutter project.

## Getting Started

This project is a starting point for a Flutter application.

A few resources to get you started if this is your first Flutter project:

- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)

For help getting started with Flutter development, view the
[online documentation](https://docs.flutter.dev/), which offers tutorials,
samples, guidance on mobile development, and a full API reference.
