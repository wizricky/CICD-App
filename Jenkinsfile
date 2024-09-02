pipeline {
    agent any

    environment {
        DOCKERHUB_CREDENTIALS = credentials('dockerhub')
        DOCKER_IMAGE = "ristekimov/cicdapp"
        REGISTRY = "docker.io"
    }

    stages {
        stage('Checkout') {
            steps {
                // Checkout code from your Git repository
                checkout scm
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    // Build the Docker image
                    def image = docker.build("${DOCKER_IMAGE}:${env.BUILD_ID}")
                    // Tag the image for Docker Hub
                    image.tag("${DOCKER_IMAGE}:latest")
                }
            }
        }

        stage('Push Docker Image to DockerHub') {
            steps {
                script {
                    // Log in to DockerHub and push the image
                    docker.withRegistry("https://${REGISTRY}", DOCKERHUB_CREDENTIALS) {
                        // Push both the versioned and latest tags
                        docker.image("${DOCKER_IMAGE}:${env.BUILD_ID}").push()
                        docker.image("${DOCKER_IMAGE}:latest").push()
                    }
                }
            }
        }
    }

    post {
        always {
            // Clean up the local Docker image after pushing
            sh 'docker rmi ${DOCKER_IMAGE}:${env.BUILD_ID} || true'
            sh 'docker rmi ${DOCKER_IMAGE}:latest || true'
        }
    }
}
