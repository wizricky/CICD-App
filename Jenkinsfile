pipeline {
    agent any

    def image
    environment {
        DOCKERHUB_CREDENTIALS = credentials('dockerhub')
        DOCKER_IMAGE = "ristekimov/cicdapp"
        REGISTRY = "https://registry.hub.docker.com"
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
                    image = docker.build("${DOCKER_IMAGE}:${env.BUILD_ID}")
                }
            }
        }

        stage('Push Docker Image to DockerHub') {
            steps {
                script {
                    // Log in to DockerHub and push the image
                    docker.withRegistry("https://${REGISTRY}", DOCKERHUB_CREDENTIALS) {
                        // Push both the versioned and latest tags
                        image.push("${DOCKER_IMAGE}:${env.BUILD_ID}")
                        image.push("${DOCKER_IMAGE}:latest")
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
