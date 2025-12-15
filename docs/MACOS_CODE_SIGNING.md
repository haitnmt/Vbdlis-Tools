# macOS Code Signing Configuration

Hướng dẫn cấu hình code signing cho macOS build trên GitHub Actions.

## Tại sao cần Code Signing?

- **Unsigned apps**: macOS Gatekeeper sẽ chặn và hiển thị cảnh báo "App is damaged"
- **Signed apps**: Người dùng có thể cài đặt dễ dàng hơn mà không cần chạy lệnh `xattr`
- **Notarized apps**: Được Apple xác thực, mức độ tin cậy cao nhất

## Yêu cầu

1. **Apple Developer Account** ($99/năm)
   - Đăng ký tại: https://developer.apple.com/programs/

2. **Developer ID Application Certificate**
   - Dùng để ký code cho distribution bên ngoài Mac App Store

3. **App-Specific Password** (cho notarization)
   - Tạo tại: https://appleid.apple.com/account/manage

## Bước 1: Tạo Certificate (.p12)

### Trên máy Mac:

1. Mở **Keychain Access**
2. Menu: **Keychain Access** → **Certificate Assistant** → **Request a Certificate from a Certificate Authority**
3. Điền thông tin:
   - Email: email của Apple Developer Account
   - Common Name: tên của bạn
   - Chọn: **Saved to disk**
4. Lưu file `CertificateSigningRequest.certSigningRequest`

5. Truy cập [Apple Developer Certificates](https://developer.apple.com/account/resources/certificates/list)
6. Nhấn **+** để tạo certificate mới
7. Chọn **Developer ID Application**
8. Upload file `.certSigningRequest` vừa tạo
9. Download certificate (file `.cer`)

10. Double-click file `.cer` để import vào Keychain Access

11. Trong Keychain Access:
    - Tìm certificate vừa import (có tên "Developer ID Application: ...")
    - Right-click → **Export**
    - Chọn format: **Personal Information Exchange (.p12)**
    - Đặt password (lưu lại password này)
    - Lưu file `.p12`

## Bước 2: Convert Certificate sang Base64

```bash
# Trên mác Mac hoặc Linux
base64 -i YourCertificate.p12 -o certificate-base64.txt

# Hoặc trên macOS
base64 -i YourCertificate.p12 | pbcopy  # Copy trực tiếp vào clipboard
```

## Bước 3: Tạo App-Specific Password

1. Truy cập https://appleid.apple.com/account/manage
2. Đăng nhập với Apple ID (của Developer Account)
3. Phần **Security** → **App-Specific Passwords**
4. Click **Generate an app-specific password**
5. Đặt tên (ví dụ: "GitHub Actions Notarization")
6. Lưu lại password (format: xxxx-xxxx-xxxx-xxxx)

## Bước 4: Cấu hình GitHub Secrets

Truy cập repository trên GitHub:
**Settings** → **Secrets and variables** → **Actions** → **New repository secret**

Thêm các secrets sau:

### 1. MACOS_CERTIFICATE
- **Name**: `MACOS_CERTIFICATE`
- **Value**: Nội dung file `certificate-base64.txt` (toàn bộ chuỗi base64)
- **Bắt buộc**: Có

### 2. MACOS_CERTIFICATE_PWD
- **Name**: `MACOS_CERTIFICATE_PWD`
- **Value**: Password bạn đã đặt khi export file `.p12`
- **Bắt buộc**: Có

### 3. MACOS_KEYCHAIN_PWD
- **Name**: `MACOS_KEYCHAIN_PWD`
- **Value**: Một password bất kỳ (dùng cho temporary keychain, ví dụ: `actions-keychain-pwd`)
- **Bắt buộc**: Có

### 4. MACOS_NOTARIZATION_APPLE_ID
- **Name**: `MACOS_NOTARIZATION_APPLE_ID`
- **Value**: Email của Apple Developer Account
- **Bắt buộc**: Không (nhưng khuyến nghị cho notarization)

### 5. MACOS_NOTARIZATION_TEAM_ID
- **Name**: `MACOS_NOTARIZATION_TEAM_ID`
- **Value**: Team ID (tìm ở [Membership page](https://developer.apple.com/account#!/membership/))
- **Bắt buộc**: Không (nhưng khuyến nghị cho notarization)

### 6. MACOS_NOTARIZATION_PWD
- **Name**: `MACOS_NOTARIZATION_PWD`
- **Value**: App-Specific Password (tạo ở Bước 3)
- **Bắt buộc**: Không (nhưng khuyến nghị cho notarization)

## Kiểm tra cấu hình

Sau khi cấu hình xong:

1. Push code và tạo release tag
2. GitHub Actions sẽ build macOS với code signing
3. Kiểm tra logs để đảm bảo signing thành công:
   - ✅ "App signed successfully"
   - ✅ "App notarized successfully" (nếu có notarization)

## Lưu ý

### Code Signing (Tối thiểu)
Chỉ cần 3 secrets đầu tiên:
- `MACOS_CERTIFICATE`
- `MACOS_CERTIFICATE_PWD`
- `MACOS_KEYCHAIN_PWD`

App sẽ được ký nhưng chưa notarized. Người dùng vẫn cần:
- Right-click → Open lần đầu
- Hoặc chạy: `xattr -cr "/Applications/VBDLIS Tools.app"`

### Notarization (Khuyến nghị)
Thêm 3 secrets cho notarization:
- `MACOS_NOTARIZATION_APPLE_ID`
- `MACOS_NOTARIZATION_TEAM_ID`
- `MACOS_NOTARIZATION_PWD`

App sẽ được ký + notarized. Người dùng có thể:
- Mở trực tiếp bằng double-click
- Không cần chạy lệnh `xattr`

### Không có Code Signing (Hiện tại)
Nếu không cấu hình secrets:
- App sẽ unsigned
- Người dùng PHẢI chạy: `xattr -cr "/Applications/VBDLIS Tools.app"`
- Xem hướng dẫn trong file README.txt kèm theo DMG

## Troubleshooting

### Certificate không hợp lệ
- Đảm bảo certificate chưa hết hạn
- Đảm bảo đã export cả private key khi tạo `.p12`

### Notarization thất bại
- Kiểm tra App-Specific Password còn hiệu lực
- Kiểm tra Team ID đúng
- Đảm bảo Apple Developer Account còn active

### Build thất bại
- Kiểm tra base64 encoding đúng (không có line breaks ngoài ý muốn)
- Kiểm tra password chính xác
- Xem logs chi tiết trong GitHub Actions

## Tham khảo

- [Apple Code Signing Guide](https://developer.apple.com/support/code-signing/)
- [Notarizing macOS Software](https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution)
- [Creating App-Specific Passwords](https://support.apple.com/en-us/HT204397)
