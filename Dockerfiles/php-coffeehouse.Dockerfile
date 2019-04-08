FROM php:7.0-apache

ARG WEB_APP_PATH

RUN apt-get update \
# Install base packages
    && apt-get install -y \
        git \
        libmcrypt-dev \
        libmemcached-dev \
        mysql-client \
        valgrind \
        vim \
        wget \
        unzip \
        zlib1g-dev \
    && pecl install redis \
    && docker-php-ext-install mcrypt \
    && docker-php-ext-enable redis \
    && pecl install memcached \
    && docker-php-ext-enable mcrypt \
    && docker-php-ext-enable memcached \
    && docker-php-ext-install mysqli pdo pdo_mysql \
    && docker-php-ext-enable mysqli \
    && docker-php-ext-enable pdo \
    && docker-php-ext-enable pdo_mysql \
    && docker-php-ext-install pcntl \
    && docker-php-ext-enable pcntl \
    && docker-php-source delete \
# Install composer
    && php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');" \
    && php composer-setup.php  --install-dir="/usr/bin" --filename=composer \
    && php -r "unlink('composer-setup.php');" \
    && composer self-update \
# Remove installation cache
    && rm -rf /var/lib/apt/lists/*

COPY Dockerfiles/php/apache2.conf /etc/apache2/apache2.conf
COPY Dockerfiles/php/default.conf /etc/apache2/sites-available/000-default.conf

COPY Dockerfiles/php/packages/datadog-php-tracer_0.12.2-beta_amd64.deb datadog-php-tracer.deb
RUN dpkg -i datadog-php-tracer.deb

RUN a2enmod rewrite

COPY ${WEB_APP_PATH} /var/www
WORKDIR /var/www

ENV COMPOSER_ALLOW_SUPERUSER 1

RUN composer update

RUN chmod -R 777 app/storage/

CMD [ "apache2-foreground" ]
