# Firebase Setup Guide

Hướng dẫn thiết lập Firebase cho ứng dụng Traffic Sign Mobile.

## 1. Tạo Firebase Project

1. Truy cập [Firebase Console](https://console.firebase.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Bật các tính năng cần thiết:
   - **Cloud Messaging (FCM)** - Cho push notifications
   - **Analytics** (tùy chọn) - Cho phân tích hành vi người dùng

## 2. Android Setup

### 2.1. Thêm Android App vào Firebase

1. Trong Firebase Console, chọn **Add app** > **Android**
2. Nhập package name: `com.trafficsign.app` (hoặc package name của bạn)
3. Download file `google-services.json`
4. Đặt file vào: `android/app/google-services.json`

### 2.2. Cấu hình Gradle

**File: `android/build.gradle.kts`**
```kotlin
buildscript {
    dependencies {
        // ... existing dependencies
        classpath("com.google.gms:google-services:4.4.0")
    }
}
```

**File: `android/app/build.gradle.kts`**
```kotlin
plugins {
    // ... existing plugins
    id("com.google.gms.google-services")
}

dependencies {
    // ... existing dependencies
    implementation(platform("com.google.firebase:firebase-bom:32.7.0"))
    implementation("com.google.firebase:firebase-messaging")
}
```

## 3. iOS Setup

### 3.1. Thêm iOS App vào Firebase

1. Trong Firebase Console, chọn **Add app** > **iOS**
2. Nhập Bundle ID: `com.trafficsign.app` (hoặc bundle ID của bạn)
3. Download file `GoogleService-Info.plist`
4. Đặt file vào: `ios/Runner/GoogleService-Info.plist`

### 3.2. Cấu hình Xcode

1. Mở `ios/Runner.xcworkspace` trong Xcode
2. Thêm `GoogleService-Info.plist` vào project (nếu chưa có)
3. Đảm bảo file được thêm vào target "Runner"

### 3.3. Cấu hình Capabilities

1. Trong Xcode, chọn target "Runner"
2. Vào tab "Signing & Capabilities"
3. Thêm capability "Push Notifications"
4. Thêm capability "Background Modes" và bật "Remote notifications"

### 3.4. Cấu hình Podfile

**File: `ios/Podfile`**
```ruby
platform :ios, '12.0'

target 'Runner' do
  use_frameworks!
  use_modular_headers!

  # ... existing pods
  
  pod 'Firebase/Messaging'
  pod 'Firebase/Analytics'
end
```

Sau đó chạy:
```bash
cd ios
pod install
```

## 4. Kiểm tra Setup

Sau khi setup xong, chạy ứng dụng và kiểm tra log:

```dart
// FCM Token sẽ được in ra trong console
// Nếu thấy token, nghĩa là setup thành công
```

## 5. Backend Integration

Để nhận push notifications từ backend, bạn cần:

1. Lấy FCM token từ app:
```dart
final token = await FirebaseService.instance.getFCMToken();
```

2. Gửi token lên backend API (khi user login):
```dart
// POST /api/users/{userId}/fcm-token
// Body: { "token": "fcm_token_here" }
```

3. Backend sẽ sử dụng FCM token để gửi notifications đến device

## 6. Testing Push Notifications

### 6.1. Test từ Firebase Console

1. Vào Firebase Console > Cloud Messaging
2. Click "Send test message"
3. Nhập FCM token từ app
4. Gửi test message

### 6.2. Test từ Backend

Backend có thể gửi notification qua FCM API:
- Endpoint: `https://fcm.googleapis.com/v1/projects/{project_id}/messages:send`
- Authentication: Service Account JSON key

## 7. Troubleshooting

### Android: Không nhận được notifications
- Kiểm tra `google-services.json` đã đặt đúng vị trí
- Kiểm tra Gradle đã được cấu hình đúng
- Kiểm tra app đã được cấp quyền notifications

### iOS: Không nhận được notifications
- Kiểm tra `GoogleService-Info.plist` đã được thêm vào project
- Kiểm tra Capabilities đã được bật
- Kiểm tra APNs certificates đã được cấu hình trong Firebase Console

### Token không được tạo
- Kiểm tra Firebase đã được initialize trong `bootstrap.dart`
- Kiểm tra log để xem lỗi cụ thể
- Đảm bảo app đã được cấp quyền notifications

## 8. Production Checklist

- [ ] Firebase project đã được tạo
- [ ] `google-services.json` đã được thêm vào Android
- [ ] `GoogleService-Info.plist` đã được thêm vào iOS
- [ ] Gradle/Podfile đã được cấu hình
- [ ] FCM token được gửi lên backend khi login
- [ ] Backend có thể gửi notifications qua FCM
- [ ] Test notifications hoạt động trên cả Android và iOS

## Notes

- Firebase sẽ fail gracefully nếu chưa được cấu hình (app vẫn chạy được)
- FCM token sẽ tự động refresh khi cần
- Background messages được handle tự động qua `firebaseMessagingBackgroundHandler`

