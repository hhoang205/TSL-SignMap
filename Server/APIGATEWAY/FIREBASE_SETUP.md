# Firebase Authentication Setup Guide

Hướng dẫn chi tiết để cấu hình Firebase Authentication cho API Gateway.

## Prerequisites

1. Firebase project đã được tạo
2. Firebase Authentication đã được enable trong Firebase Console
3. Service account credentials (optional, nhưng recommended)

## Bước 1: Lấy Firebase Project ID

1. Vào [Firebase Console](https://console.firebase.google.com/)
2. Chọn project của bạn
3. Vào Project Settings (⚙️ icon)
4. Copy **Project ID** (không phải Project Number)

## Bước 2: Tạo Service Account (Recommended)

1. Vào Firebase Console → Project Settings → Service Accounts
2. Click "Generate new private key"
3. Download file JSON (ví dụ: `firebase-service-account.json`)
4. Lưu file vào thư mục an toàn (không commit vào Git!)

## Bước 3: Cấu hình API Gateway

### Option 1: Sử dụng Service Account File

Cập nhật `appsettings.json`:

```json
{
  "Gateway": {
    "Firebase": {
      "ProjectId": "your-firebase-project-id",
      "ServiceAccountPath": "./firebase-service-account.json",
      "Enabled": true,
      "AllowJwtFallback": true
    }
  }
}
```

### Option 2: Sử dụng Environment Variable

1. Set environment variable `FIREBASE_CREDENTIALS` với nội dung JSON của service account:

```bash
# Windows PowerShell
$env:FIREBASE_CREDENTIALS = Get-Content firebase-service-account.json -Raw

# Linux/Mac
export FIREBASE_CREDENTIALS=$(cat firebase-service-account.json)
```

2. Cập nhật `appsettings.json` (không cần `ServiceAccountPath`):

```json
{
  "Gateway": {
    "Firebase": {
      "ProjectId": "your-firebase-project-id",
      "Enabled": true,
      "AllowJwtFallback": true
    }
  }
}
```

### Option 3: Default Credentials (GCP only)

Nếu chạy trên GCP (Cloud Run, GKE, Compute Engine), có thể sử dụng default credentials:

```json
{
  "Gateway": {
    "Firebase": {
      "ProjectId": "your-firebase-project-id",
      "Enabled": true,
      "AllowJwtFallback": true
    }
  }
}
```

## Bước 4: Test Firebase Authentication

### 1. Lấy Firebase ID Token từ client

Ví dụ với JavaScript (Firebase SDK):

```javascript
import { getAuth, signInWithEmailAndPassword } from "firebase/auth";

const auth = getAuth();
const userCredential = await signInWithEmailAndPassword(auth, email, password);
const idToken = await userCredential.user.getIdToken();

// Sử dụng token trong API requests
fetch('https://your-api-gateway/api/users', {
  headers: {
    'Authorization': `Bearer ${idToken}`
  }
});
```

### 2. Test với curl

```bash
curl -H "Authorization: Bearer YOUR_FIREBASE_ID_TOKEN" \
     http://localhost:5000/api/users
```

### 3. Kiểm tra logs

API Gateway sẽ log thông tin về Firebase authentication:

```
[INFO] Firebase Authentication enabled for project: your-project-id
[DEBUG] Firebase token authenticated successfully for user: abc123xyz
```

## Claims Mapping

Firebase tokens được convert thành claims như sau:

| Firebase Claim | ASP.NET Core Claim | Description |
|----------------|-------------------|-------------|
| `uid` | `ClaimTypes.NameIdentifier` | User ID |
| `name` | `ClaimTypes.Name` | Display name |
| `email` | `ClaimTypes.Email` | Email address |
| `email_verified` | `email_verified` | Email verification status |
| `role` (custom) | `ClaimTypes.Role` | User role |
| `uid` | `firebase_uid` | Firebase UID |

## Custom Claims

Để thêm custom claims (như role) vào Firebase tokens, sử dụng Firebase Admin SDK:

```csharp
// Example: Set custom claim for user
var claims = new Dictionary<string, object>
{
    { "role", "Admin" }
};
await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
```

## Security Best Practices

1. **Never commit service account files to Git**
   - Thêm `firebase-service-account.json` vào `.gitignore`
   - Sử dụng environment variables hoặc secret management trong production

2. **Use environment variables in production**
   ```bash
   # Docker
   docker run -e FIREBASE_CREDENTIALS="$(cat firebase-service-account.json)" ...
   
   # Kubernetes
   kubectl create secret generic firebase-credentials \
     --from-file=credentials.json=firebase-service-account.json
   ```

3. **Restrict service account permissions**
   - Chỉ cấp quyền cần thiết cho service account
   - Sử dụng IAM roles với least privilege principle

4. **Enable Firebase App Check** (optional)
   - Giúp bảo vệ API khỏi abuse
   - Cấu hình trong Firebase Console

## Troubleshooting

### Error: "Firebase ProjectId must be configured"
- Kiểm tra `ProjectId` trong `appsettings.json`
- Đảm bảo `Enabled: true` nếu muốn sử dụng Firebase

### Error: "Failed to initialize Firebase Admin SDK"
- Kiểm tra service account file path
- Kiểm tra environment variable `FIREBASE_CREDENTIALS`
- Verify service account có quyền truy cập Firebase project

### Token validation fails
- Kiểm tra token chưa expired
- Verify token được issue từ đúng Firebase project
- Kiểm tra Firebase Authentication đã được enable trong Firebase Console

### Both JWT and Firebase tokens fail
- Nếu `AllowJwtFallback: true`, hệ thống sẽ thử cả 2 loại tokens
- Kiểm tra logs để xem token nào đang được sử dụng
- Verify configuration trong `appsettings.json`

## Migration từ JWT sang Firebase

1. Enable Firebase với `AllowJwtFallback: true` (cho phép cả 2 loại tokens)
2. Update clients để sử dụng Firebase tokens
3. Monitor logs để đảm bảo Firebase authentication hoạt động
4. Sau khi tất cả clients đã migrate, set `AllowJwtFallback: false` (optional)

## References

- [Firebase Admin SDK Documentation](https://firebase.google.com/docs/admin/setup)
- [Firebase Authentication](https://firebase.google.com/docs/auth)
- [Custom Claims](https://firebase.google.com/docs/auth/admin/custom-claims)

