CREATE DATABASE IF NOT EXISTS badminton_shop
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE badminton_shop;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS wishlist_items;
DROP TABLE IF EXISTS wishlists;
DROP TABLE IF EXISTS reviews;
DROP TABLE IF EXISTS order_status_history;
DROP TABLE IF EXISTS shipments;
DROP TABLE IF EXISTS payments;
DROP TABLE IF EXISTS order_details;
DROP TABLE IF EXISTS cart_items;
DROP TABLE IF EXISTS carts;
DROP TABLE IF EXISTS product_specs;
DROP TABLE IF EXISTS product_images;
DROP TABLE IF EXISTS product_variants;
DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS coupons;
DROP TABLE IF EXISTS products;
DROP TABLE IF EXISTS brands;
DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS addresses;
DROP TABLE IF EXISTS users;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    avatar_url VARCHAR(255),
    gender ENUM('male', 'female', 'other') DEFAULT 'other',
    birthdate DATE,
    role ENUM('admin', 'staff', 'customer') DEFAULT 'customer',
    is_active BOOLEAN DEFAULT TRUE,
    email_verified BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE addresses (
    address_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    recipient_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    province VARCHAR(100),
    district VARCHAR(100),
    ward VARCHAR(100),
    address_detail TEXT,
    is_default BOOLEAN DEFAULT FALSE,
    FOREIGN KEY(user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE
);

CREATE TABLE categories (
    category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT
);

CREATE TABLE brands (
    brand_id INT AUTO_INCREMENT PRIMARY KEY,
    brand_name VARCHAR(100) NOT NULL UNIQUE,
    logo_url VARCHAR(255)
);

CREATE TABLE products (
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    category_id INT NOT NULL,
    brand_id INT NOT NULL,
    product_name VARCHAR(200) NOT NULL,
    slug VARCHAR(255) UNIQUE,
    short_description TEXT,
    description LONGTEXT,
    status ENUM('active', 'out_of_stock', 'hidden', 'discontinued') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY(category_id) REFERENCES categories(category_id),
    FOREIGN KEY(brand_id) REFERENCES brands(brand_id)
);

CREATE TABLE product_variants (
    variant_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    sku VARCHAR(100) UNIQUE,
    weight VARCHAR(20),
    grip_size VARCHAR(20),
    color VARCHAR(50),
    price DECIMAL(12,2) UNSIGNED NOT NULL,
    stock_quantity INT UNSIGNED DEFAULT 0,
    image_url VARCHAR(255),
    FOREIGN KEY(product_id)
        REFERENCES products(product_id)
        ON DELETE CASCADE
);

CREATE TABLE product_images (
    image_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    image_url VARCHAR(255) NOT NULL,
    is_main BOOLEAN DEFAULT FALSE,
    sort_order INT DEFAULT 0,
    FOREIGN KEY(product_id)
        REFERENCES products(product_id)
        ON DELETE CASCADE
);

CREATE TABLE product_specs (
    spec_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    spec_name VARCHAR(100),
    spec_value VARCHAR(255),
    FOREIGN KEY(product_id)
        REFERENCES products(product_id)
        ON DELETE CASCADE
);

CREATE TABLE carts (
    cart_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE
);

CREATE TABLE cart_items (
    cart_item_id INT AUTO_INCREMENT PRIMARY KEY,
    cart_id INT NOT NULL,
    variant_id INT NOT NULL,
    quantity INT UNSIGNED NOT NULL CHECK (quantity > 0),
    FOREIGN KEY(cart_id)
        REFERENCES carts(cart_id)
        ON DELETE CASCADE,
    FOREIGN KEY(variant_id)
        REFERENCES product_variants(variant_id)
        ON DELETE CASCADE
);

CREATE TABLE coupons (
    coupon_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(50) UNIQUE,
    discount_type ENUM('percentage', 'fixed'),
    discount_value DECIMAL(12,2) UNSIGNED NOT NULL,
    max_discount DECIMAL(12,2) UNSIGNED,
    min_order_value DECIMAL(12,2) UNSIGNED DEFAULT 0.00,
    start_date DATETIME,
    end_date DATETIME,
    usage_limit INT UNSIGNED,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE orders (
    order_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    coupon_id INT NULL,
    shipping_recipient_name VARCHAR(100) NOT NULL,
    shipping_phone VARCHAR(20) NOT NULL,
    shipping_province VARCHAR(100) NOT NULL,
    shipping_district VARCHAR(100) NOT NULL,
    shipping_ward VARCHAR(100) NOT NULL,
    shipping_address_detail TEXT NOT NULL,
    total_amount DECIMAL(12,2) UNSIGNED DEFAULT 0.00,
    shipping_fee DECIMAL(12,2) UNSIGNED DEFAULT 0.00,
    discount_amount DECIMAL(12,2) UNSIGNED DEFAULT 0.00,
    final_amount DECIMAL(12,2) UNSIGNED DEFAULT 0.00,
    status ENUM('pending', 'confirmed', 'packing', 'shipping', 'delivered', 'cancelled') DEFAULT 'pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(user_id) REFERENCES users(user_id),
    FOREIGN KEY(coupon_id) REFERENCES coupons(coupon_id) ON DELETE SET NULL
);

CREATE TABLE order_details (
    order_detail_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    variant_id INT NOT NULL,
    quantity INT UNSIGNED NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(12,2) UNSIGNED NOT NULL,
    subtotal DECIMAL(12,2) UNSIGNED NOT NULL,
    FOREIGN KEY(order_id)
        REFERENCES orders(order_id)
        ON DELETE CASCADE,
    FOREIGN KEY(variant_id)
        REFERENCES product_variants(variant_id)
);

CREATE TABLE payments (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT UNIQUE,
    payment_method ENUM('cod', 'vnpay', 'momo'),
    payment_status ENUM('pending', 'paid', 'failed') DEFAULT 'pending',
    transaction_code VARCHAR(100),
    paid_at DATETIME,
    FOREIGN KEY(order_id)
        REFERENCES orders(order_id)
        ON DELETE CASCADE
);

CREATE TABLE shipments (
    shipment_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT UNIQUE,
    tracking_number VARCHAR(100) UNIQUE,
    courier VARCHAR(100),
    shipped_date DATETIME,
    delivered_date DATETIME,
    status ENUM('preparing', 'shipping', 'delivered') DEFAULT 'preparing',
    FOREIGN KEY(order_id)
        REFERENCES orders(order_id)
        ON DELETE CASCADE
);

CREATE TABLE order_status_history (
    history_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    status VARCHAR(50),
    note TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(order_id)
        REFERENCES orders(order_id)
        ON DELETE CASCADE
);

CREATE TABLE reviews (
    review_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    rating INT CHECK(rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id, product_id),
    FOREIGN KEY(user_id) REFERENCES users(user_id),
    FOREIGN KEY(product_id) REFERENCES products(product_id) ON DELETE CASCADE
);

CREATE TABLE wishlists (
    wishlist_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT UNIQUE,
    FOREIGN KEY(user_id)
        REFERENCES users(user_id)
        ON DELETE CASCADE
);

CREATE TABLE wishlist_items (
    wishlist_item_id INT AUTO_INCREMENT PRIMARY KEY,
    wishlist_id INT NOT NULL,
    product_id INT NOT NULL,
    FOREIGN KEY(wishlist_id)
        REFERENCES wishlists(wishlist_id)
        ON DELETE CASCADE,
    FOREIGN KEY(product_id)
        REFERENCES products(product_id)
        ON DELETE CASCADE
);
