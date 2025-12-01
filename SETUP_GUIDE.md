# Hướng Dẫn Cài Đặt và Thiết Lập

## Giới thiệu
Hướng dẫn này sẽ giúp bạn cài đặt và chạy dự án TimViecLam_BE_2, bao gồm các bước thiết lập cơ sở dữ liệu, áp dụng di chuyển, và thêm dữ liệu mẫu.

## Yêu Cầu và Cài Đặt
1. Cài đặt [Node.js](https://nodejs.org/)
2. Cài đặt [MySQL](https://www.mysql.com/)
3. Cài đặt [Docker](https://www.docker.com/) (Tùy chọn)

## Chạy Dự Án
1. **Clone repository:**
   ```bash
   git clone https://github.com/hgmducc/TimViecLam_BE_2.git
   cd TimViecLam_BE_2
   ```

2. **Cài đặt các phụ thuộc:**
   ```bash
   npm install
   ```

## Thiết Lập Cơ Sở Dữ Liệu
1. **Tạo cơ sở dữ liệu:**
   - Truy cập MySQL và tạo một cơ sở dữ liệu mới:
     ```sql
     CREATE DATABASE timvieclam;
     ```

2. **Cấu hình kết nối cơ sở dữ liệu:**
   - Chỉnh sửa tệp `.env` với thông tin đăng nhập MySQL của bạn.

## Áp Dụng Di Chuyển
- Chạy lệnh sau để áp dụng các di chuyển:
  ```bash
  npx sequelize-cli db:migrate
  ```

## Thêm Dữ Liệu Mẫu
- Chạy lệnh sau để thêm dữ liệu mẫu:
  ```bash
  npx sequelize-cli db:seed:all
  ```

## Khắc Phục Vấn Đề Thường Gặp
- **Lỗi kết nối cơ sở dữ liệu:**
  - Kiểm tra thông tin đăng nhập có đúng không.
- **Lỗi khi áp dụng di chuyển:**
  - Đảm bảo rằng mọi di chuyển đã được định nghĩa chính xác và không có lỗi cú pháp.

## Kết Luận
Hy vọng hướng dẫn này sẽ giúp bạn thiết lập thành công dự án của mình. Nếu gặp vấn đề, hãy kiểm tra các bước trên hoặc tạo một vấn đề trên GitHub.
