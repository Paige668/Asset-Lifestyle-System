#!/bin/bash

# ============================================
# AWS EC2 Deployment Script for IT Asset Manager
# ============================================
# This script automates the deployment on Ubuntu EC2 instance

set -e  # Exit on any error

echo "🚀 Starting IT Asset Manager Deployment..."

# Update system packages
echo "📦 Updating system packages..."
sudo apt-get update -y
sudo apt-get upgrade -y

# Install Docker
echo "🐳 Installing Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    echo "✅ Docker installed successfully"
else
    echo "✅ Docker already installed"
fi

# Install Docker Compose
echo "📦 Installing Docker Compose..."
if ! command -v docker-compose &> /dev/null; then
    sudo curl -L "https://github.com/docker/compose/releases/download/v2.23.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
    echo "✅ Docker Compose installed successfully"
else
    echo "✅ Docker Compose already installed"
fi

# Clone or copy project files (adjust as needed)
echo "📥 Setting up project files..."
PROJECT_DIR="/home/ubuntu/ITAssetManager"

if [ ! -d "$PROJECT_DIR" ]; then
    echo "Please upload your project files to $PROJECT_DIR"
    mkdir -p "$PROJECT_DIR"
    # Alternatively, clone from git:
    # git clone <your-repo-url> "$PROJECT_DIR"
fi

cd "$PROJECT_DIR"

# Create .env with a random SQL Server password on first run (docker-compose.yml reads it)
echo "🔧 Configuring environment variables..."
if [ ! -f .env ]; then
    SA_PASSWORD="$(openssl rand -base64 24 | tr -d '/+=')Aa1!"
    echo "MSSQL_SA_PASSWORD=$SA_PASSWORD" > .env
    chmod 600 .env
    echo "✅ Generated .env with a random SQL Server password"
else
    echo "✅ Using existing .env"
fi

# Build and start containers
echo "🏗️ Building and starting Docker containers..."
sudo docker-compose down || true
sudo docker-compose up -d --build

# Wait for services to be healthy
echo "⏳ Waiting for services to start..."
sleep 30

# Check service status
echo "📊 Checking service status..."
sudo docker-compose ps

echo ""
echo "============================================"
echo "✅ Deployment Complete!"
echo "============================================"
echo ""
echo "🌐 Access your application:"
echo "  - Frontend: http://$(curl -s http://169.254.169.254/latest/meta-data/public-ipv4):4200"
echo "  - Backend API: http://$(curl -s http://169.254.169.254/latest/meta-data/public-ipv4):8080"
echo "  - Swagger UI: http://$(curl -s http://169.254.169.254/latest/meta-data/public-ipv4):8080/swagger"
echo ""
echo "📝 Useful commands:"
echo "  - View logs: sudo docker-compose logs -f"
echo "  - Restart: sudo docker-compose restart"
echo "  - Stop: sudo docker-compose down"
echo ""
