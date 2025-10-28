#!/bin/bash

# SSL setup script for Comaiz Admin VPS
# Replace the following variables with your actual values
DOMAIN="comaiz.co.uk"
EMAIL="danbowker@gmail.com"

echo "Setting up SSL certificates for $DOMAIN..."

# Create directories
sudo mkdir -p /etc/letsencrypt
sudo mkdir -p /var/www/certbot

# Create initial nginx config for HTTP-only (for certificate generation)
sudo tee /etc/nginx/sites-available/comaiz-temp > /dev/null <<EOF
server {
    listen 80;
    server_name $DOMAIN www.$DOMAIN;
    
    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
EOF

# Enable the temporary config
sudo ln -sf /etc/nginx/sites-available/comaiz-temp /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t && sudo systemctl reload nginx

# Install certbot if not already installed
if ! command -v certbot &> /dev/null; then
    sudo apt update
    sudo apt install -y certbot python3-certbot-nginx
fi

# Generate SSL certificate
sudo certbot certonly --webroot \
    --webroot-path=/var/www/certbot \
    --email $EMAIL \
    --agree-tos \
    --no-eff-email \
    -d $DOMAIN \
    -d www.$DOMAIN

# Copy the SSL-enabled nginx config
sudo cp ./nginx/nginx.conf /etc/nginx/sites-available/comaiz-ssl

# Update the config with your actual domain
sudo sed -i "s/your-domain.com/$DOMAIN/g" /etc/nginx/sites-available/comaiz-ssl

# Enable SSL config
sudo ln -sf /etc/nginx/sites-available/comaiz-ssl /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/comaiz-temp

# Test and reload nginx
sudo nginx -t && sudo systemctl reload nginx

# Set up automatic certificate renewal
echo "Setting up automatic certificate renewal..."
(sudo crontab -l 2>/dev/null; echo "0 12 * * * /usr/bin/certbot renew --quiet && systemctl reload nginx") | sudo crontab -

echo "SSL setup complete!"
echo "Your site should now be accessible at https://$DOMAIN"