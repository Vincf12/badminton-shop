CREATE DATABASE IF NOT EXISTS badminton_shop
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE badminton_shop;

-- Bảng người dùng
CREATE TABLE USERS (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    birthdate DATE,
    phone VARCHAR(20),
    address TEXT,
    role ENUM('customer','admin') DEFAULT 'customer',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Danh mục sản phẩm
CREATE TABLE CATEGORIES (
    category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL,
    description TEXT
);

-- Sản phẩm
CREATE TABLE PRODUCTS (
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    category_id INT,
    product_name VARCHAR(150) NOT NULL,
    brand VARCHAR(100),
    price DECIMAL(12,2) NOT NULL,
    stock INT DEFAULT 0,
    image_url VARCHAR(255),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES CATEGORIES(category_id)
);

-- Đơn hàng
CREATE TABLE ORDERS (
    order_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status ENUM('pending','confirmed','shipped','delivered','cancelled') DEFAULT 'pending',
    total_amount DECIMAL(12,2),
    FOREIGN KEY (user_id) REFERENCES USERS(user_id)
);

-- Chi tiết đơn hàng
CREATE TABLE ORDER_DETAILS (
    order_detail_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT,
    product_id INT,
    quantity INT NOT NULL,
    price DECIMAL(12,2) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES ORDERS(order_id),
    FOREIGN KEY (product_id) REFERENCES PRODUCTS(product_id)
);

-- Thanh toán
CREATE TABLE PAYMENTS (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT,
    payment_method ENUM('cod','bank_transfer','credit_card') NOT NULL,
    payment_status ENUM('pending','paid','failed') DEFAULT 'pending',
    payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (order_id) REFERENCES ORDERS(order_id)
);

-- Giao hàng
CREATE TABLE SHIPMENTS (
    shipment_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT,
    tracking_number VARCHAR(100),
    courier VARCHAR(100),
    shipped_date TIMESTAMP NULL,
    delivered_date TIMESTAMP NULL,
    status ENUM('preparing','shipping','delivered') DEFAULT 'preparing',
    FOREIGN KEY (order_id) REFERENCES ORDERS(order_id)
);

-- Đánh giá sản phẩm
CREATE TABLE REVIEWS (
    review_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    product_id INT,
    rating INT CHECK(rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES USERS(user_id),
    FOREIGN KEY (product_id) REFERENCES PRODUCTS(product_id)
);
