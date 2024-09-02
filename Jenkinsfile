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
                    docker.build("$DOCKER_IMAGE:${env.BUILD_ID}")
                }
            }
        }

        stage('Push Docker Image to DockerHub') {
            steps {
                script {
                    // Log in to DockerHub and push the image
                    docker.withRegistry("https://$REGISTRY", DOCKERHUB_CREDENTIALS) {
                        docker.image("$DOCKER_IMAGE:${env.BUILD_ID}").push()
                    }
                }
            }
        }
    }

    post {
        always {
            // Clean up the local Docker image after pushing
            sh 'docker rmi $DOCKER_IMAGE:${env.BUILD_ID} || true'
        }
    }
}
