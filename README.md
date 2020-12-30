# k8s-istio

## Install Minikube
https://minikube.sigs.k8s.io/docs/start/

```sh
brew install minikube
```

## Start your cluster 

```sh
minikube start
```

## Start the Minikube Dashboard

```
minikube dashboard
```
## Start working with k8s
1. Create a docker image

    * To test your App v1 image:
    ```
    eval $(minikube docker-env)
    docker build -f ./src/app/Dockerfile --build-arg VERSION=1.0.0 -t local/app:1.0.0 ./src/app
    docker run --rm -it -p 8080:80 local/app:1.0.0
    curl localhost:8080/echo 
    ```

2. Create a deployment
    * See ./src/deployments/deployment-app-v1.yaml
    * Apply your deployment:
    ```sh
    kubectl apply -f ./src/deployments/deployment-app-v1.yaml
    ```
    * To delete the deployment, run:
    ```
    kubectl delete -f ./src/deployments/deployment-app-v1.yaml
    ```

    * See your pods:
    ```
    kubectl get pods
    ```
    * Search for your pod IP:
    ```
    kubectl get pods -l run=appv1 -o wide
    ```
    * Start the `kubectl proxy`
    ```
    kubectl proxy
    ```
    * Proxy to your pod:
    ```
    curl http://localhost:8001/api/v1/namespaces/default/pods/$POD_NAME/proxy/echo
    ```
    You should see the message `this is the app version 1.0` 
3. Create a service
    * See ./src/services/service-app.yaml
    * Apply your service:
    ```sh
    kubectl apply -f ./src/services/service-app.yaml
    ```

    * See your services:
    ```
    kubectl get services
    ```
    * Call your service:
    ```
    curl $(minikube ip):30007/echo
    ```
    You should see the message `this is the app version 1.0` 