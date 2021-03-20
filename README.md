<p align="center"><img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/logo.png" height="180"/></p>

# ĐỒ ÁN 1: FOOD RECIPES

<p align="left">
<img src="https://img.shields.io/badge/version-1.0.0-blue">
<img src="https://img.shields.io/badge/platforms-Windows-orange.svg">
</p>

### Thông tin nhóm

|       Họ và tên      |   MSSV   | Email                           | 
|----------------------|:--------:|---------------------------------|
| Trương Quốc Thuận    | 18120583 | lalalag129@gmail.com            |
| Hoàng Thị Thùy Trang | 18120605 | hoangthithuytrang1707@gmail.com |
| Lê Nhật Tuấn         | 18120632 | nhattuannhat99@gmail.com        |

### Các chức năng đã làm được
**1. Splash Screen (0.5 điểm)**
- Hiển thị thông tin chào mừng mỗi khi ứng dụng khởi chạy.
- Mỗi lần hiện ngẫu nhiên một thông tin thú vị về món ăn / loại đồ ăn.
- Cho phép chọn check “Không hiện hộp thoại này mỗi khi khởi động”. Từ nay về sau đi thẳng vào màn hình HomeScreen luôn.

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/splash.png" width="516"/>
</div>

**2. HomeScreen (3 điểm)**
- Liệt kê danh sách các món ăn được ưa thích.
- Liệt kê toàn bộ danh sách món ăn có phân trang.
- Cần lưu lại thông tin phân trang cho mỗi lần mở app lên. Ví dụ có lần thì coi 5 sản phẩm mỗi trang, có lần 10 sản phẩm mỗi trang. Có lần đang sắp xếp tăng dần theo tên, có lần sắp xếp giảm dần theo ngày tạo hoặc ngày cập nhật.
- Có thể xem danh sách các món ăn ưa thích (được ghim lại), thêm và loại bỏ món ăn vào danh sách ưa thích này.

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Home_1.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Home_2.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Home_3.png" width="1080"/>
</div>

**3. SearchScreen (2 điểm)**
- Tìm kiếm món ăn theo tên. 
- Cảnh giới 1: Tìm chính xác mới chịu.
- Cảnh giới 2: Hỗ trợ tìm kiếm không dấu (cách dễ nhất, chuyển tất cả thành không dấu).
- Cảnh giới 3: Tìm không dấu hay có dấu hoặc có dấu chưa đúng nhưng kết quả vẫn ra và có độ ưu tiên.
- Ví dụ: Tìm với chữ “cỏ trang” vẫn có thể ra “cổ trang” và “cò trắng”.
- Cảnh giới 4: Tìm với từng từ hoặc kết hợp tạo ra tổ hợp từ các từ, có thể trong các trường khác nhau của dữ liệu.
- Cảnh giới 5: Thêm các từ khóa and, or, not.
- Cảnh giới 6: Dùng CSDL hỗ trợ sạch các cảnh giới trên (Nhóm sử dụng SQL Server Express 2019).

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Search_1.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Search_2.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Search_3.png" width="1080"/>
</div>

**4. DetailScreen (2 điểm)**
- Hiển thị chi tiết các bước nấu món ăn.
Có danh sách hh dạng carousel, có thể xem video (cục bộ hoặc online hoặc nhúng youtube đều được)

- Cảnh giới 1: Có animation
- Cảnh giới 2: Xem video từ internet

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Detail_1.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Detail_2.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Detail_3.png" width="1080"/>
</div>

**5. AddRecipe (2 điểm)**
- Cho phép người dùng tự thêm một công thức nấu ăn vào hệ thống.
- Tên món.
- Thêm các bước làm, với mỗi bước.
- Thêm mô tả bằng text.
- Thêm hình ảnh.
- Thêm video cục bộ / link video youtube.

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Add.png" width="1080"/>
</div>

**6. Others page**
- Help.
- About us.

<div>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/Help.png" width="1080"/>
<img src="https://github.com/QuocThuanTruong/FoodRecipes/blob/master/Img/About.png" width="1080"/>
</div>

### Các chức năng chưa làm được theo yêu cầu của thầy
- **Không có**

### Các chức năng, đặc điểm đặc sắc của bài tập đề nghị cộng điểm
**Shopping list**
- Hiển thị các nguyên liệu tương ứng với các món ăn được thêm vào danh sách đi chợ.
- Cho phép người dùng đánh dấu rằng nguyên liệu đã mua bằng việc tick vào nó (tick lần nữa để bỏ đánh dấu) hoặc bấm vào nút CLEAR SELECTED.
- Tìm kiếm, lọc và sắp xếp các món ăn có trong danh sách này.
- Xóa món ăn khỏi danh sách (cho phép Undo trong thời gian 3 giây).

**Thực hiện toàn bộ các cảnh giới tìm kiếm với Full-Text search của SQL Server Express 2019**

### Điểm đề nghị cho bài tập
- **Điểm đề nghị: 10đ.**
- **Điểm cộng đề nghị: 1đ.**

### Link youtube demo
> ***https://youtu.be/Rzg9LGXxTtI***

### Link file FoodRecipes.msi và hướng dẫn cài đặt
> ***https://drive.google.com/drive/folders/1VcDxOtp1tqQcl755fOMquZ4mQOeIkNrr?usp=sharing***

### Link github
> ***https://github.com/QuocThuanTruong/FoodRecipes***

### Link backup
> ***https://drive.google.com/drive/folders/12Epr40R-3YqCJABx7qLZxBFgYhelPiU2?usp=sharing***

### License
Food Recipes is available under the [MIT license](https://opensource.org/licenses/MIT) . See [LICENSE](https://github.com/QuocThuanTruong/FoodRecipes/blob/master/LICENSE) for the full license text.

