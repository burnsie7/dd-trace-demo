CREATE TABLE coffees (
    id int NOT NULL AUTO_INCREMENT,
    name varchar(255) NOT NULL,
    flavor varchar(255),
    PRIMARY KEY (id)
);

INSERT INTO coffees (id, name, flavor) VALUES
(1, 'arabic', 'soft'),
(2, 'java', 'string'),
(3, 'french', 'soft'),
(4, 'italian', 'middle'),
(5, 'american', 'strong');

CREATE TABLE favorites (
    id int NOT NULL AUTO_INCREMENT,
    coffee_id int NOT NULL,
    user_id int NOT NULL,
    PRIMARY KEY (id)
);
