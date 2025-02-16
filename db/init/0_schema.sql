CREATE TABLE `room_data_sources` (
    `id`            INT UNSIGNED        NOT NULL                PRIMARY KEY     AUTO_INCREMENT,
    `name`          VARCHAR(32)         NOT NULL                UNIQUE KEY,
    `is_active`     BOOLEAN             NOT NULL    DEFAULT 0
)   DEFAULT CHARSET=utf8mb4;

CREATE TABLE `rooms` (
    `id`                CHAR(36)        NOT NULL                    PRIMARY KEY,
    `name`              VARCHAR(255)    NOT NULL,
    `data_source_id`    INT UNSIGNED    NOT NULL,
    `alias`             VARCHAR(255)                DEFAULT NULL,
    `starts_at`         DATETIME                    DEFAULT NULL,
    `ends_at`           DATETIME                    DEFAULT NULL,
    `created_at`        DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`        DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`data_source_id`) REFERENCES `room_data_sources`(`id`)
)   DEFAULT CHARSET=utf8mb4;
ALTER TABLE `rooms` ADD INDEX `idx_name_and_time_range` (`name`, `starts_at`, `ends_at`);
ALTER TABLE `rooms` ADD INDEX `idx_time_range`          (`starts_at`, `ends_at`);

CREATE TABLE `room_admin_groups` (
    `room_id`       CHAR(36)    NOT NULL,
    `group_id`      CHAR(36)    NOT NULL    COMMENT 'traQ group uuid',
    `created_at`    DATETIME    NOT NULL    DEFAULT CURRENT_TIMESTAMP
)   DEFAULT CHARSET=utf8mb4;

CREATE TABLE `room_admin_users` (
    `room_id`       CHAR(36)    NOT NULL,
    `user_id`       CHAR(36)    NOT NULL    COMMENT 'traQ user uuid',
    `created_at`    DATETIME    NOT NULL    DEFAULT CURRENT_TIMESTAMP
)   DEFAULT CHARSET=utf8mb4;

CREATE TABLE `room_participants` (
    `room_id`       CHAR(36)        NOT NULL,
    `user_id`       CHAR(36)        NOT NULL                                COMMENT 'traQ user uuid',
    `joined_at`     DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `left_at`       DATETIME                    DEFAULT NULL,
    `created_at`    DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP
)   DEFAULT CHARSET=utf8mb4;
ALTER TABLE `room_participants` ADD INDEX `idx_room`               (`room_id`);
ALTER TABLE `room_participants` ADD INDEX `idx_user`               (`user_id`);
ALTER TABLE `room_participants` ADD INDEX `idx_room_and_join_time` (`room_id`, `joined_at`);
ALTER TABLE `room_participants` ADD INDEX `idx_user_and_join_time` (`user_id`, `joined_at`);
