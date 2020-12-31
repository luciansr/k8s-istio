# k8s-istio

# Kubernetes basics

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
    You should see the message `{"message":"This is an Echo message!","version":"1.0.0"}` 
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
    You should see the message `{"message":"This is an Echo message!","version":"1.0.0"}` 

4. Deploy a new app version alongside the old one:
    * Build a new version:
    ```
    eval $(minikube docker-env)
    docker build -f ./src/app/Dockerfile --build-arg VERSION=2.0.0 -t local/app:2.0.0 ./src/app
    ``` 
    * Apply a deployment for this new version:
    ```sh
    kubectl apply -f ./src/deployments/deployment-app-v2.yaml
    ```
    * Now execute the `curl` method again and you will see that your request go to both app versions: 
    `curl $(minikube ip):30007/echo` sometimes returns `{"message":"This is an Echo message!","version":"1.0.0"}` and sometimes `{"message":"This is an Echo message!","version":"2.0.0"}`

    This happens because our service is targeting pods with the label `run=app` and both our versions has it. The only difference is that the old version has the `version=v1` label and the new one has the `version=v2` label.

5. Do a rolling update deployment:
    * Delete all of your previous deployments:
    ```
    kubectl delete deployments --all
    ```
    * Deploy the `deployment-rolling-v1` deployment:
    ```
    kubectl apply -f ./src/deployments/deployment-rolling-v1.yaml
    ```
    * Take a look at your pods and their labels: 
    ```
    kubectl get pods --show-labels
    ```
    Something similar should come up:
    ```
    NAME                   READY   STATUS    RESTARTS   AGE   LABELS
    app-69bb7769f4-t2q2z   1/1     Running   0          93s   pod-template-hash=69bb7769f4,run=app
    ```
    Execute the `curl` method again:
    ```
    curl $(minikube ip):30007/echo
    ```
    The app will return:
    ```
    {"message":"This is an Echo message!","version":"1.0.0"}
    ```

    * Let's deploy the "same" deployment (using the same name), but changing the app version to the `2.0.0`:
    ```
    kubectl apply -f ./src/deployments/deployment-rolling-v2.yaml
    ```
    If you go fast enough, will can see the pods changing from the version `1.0.0` to the version `2.0.0` when executing `kubectl get pods`:
    ```
    NAME                   READY   STATUS        RESTARTS   AGE   LABELS
    app-6f45b9596-p5n5s    0/1     Terminating   0          32s   pod-template-hash=6f45b9596,run=app
    app-79686874fc-crzhv   1/1     Running       0          4s    pod-template-hash=79686874fc,run=app
    ```
    And when we call `curl $(minikube ip):30007/echo`. The app will return: `{"message":"This is an Echo message!","version":"2.0.0"}`

    # Enabling Istio

    https://istio.io/latest/docs/setup/getting-started/
    1. Download Istio and add it to your Path variable
    2. Install the Istio demo configuration profile on our Minikube cluster
    ```
    istioctl install --set profile=demo -y
    ```
    3. Enable istio automatic injection on the `default` namespace:
    ```
    kubectl label namespace default istio-injection=enabled
    ```

    ## Creating a canary deployment
    1. Delete all deployments
    ```
    kubectl delete deployments --all
    ```
    2. Apply the `deployment-app-v1.yaml` and `deployment-app-v2.yaml`:
    ```sh
    kubectl apply -f ./src/deployments/deployment-app-v1.yaml
    kubectl apply -f ./src/deployments/deployment-app-v2.yaml
    ```
    3. Now to configure a ingress gateway on our cluster, apply all resources inside the `istio` folder:

    ```sh
    kubectl apply -f ./src/istio
    ```
    4. Set your `INGRESS_PORT` variable by getting the values from the `istio-ingressgateway.istio-system` service node port value:
    ```
    export INGRESS_PORT=$(kubectl -n istio-system get service istio-ingressgateway -o jsonpath='{.spec.ports[?(@.name=="http2")].nodePort}')
    ```
    5. Test your ingress virtual service routing rules:
    ```
    curl $(minikube ip):$INGRESS_PORT/echo
    ```
    6. Play with the routing rules on the `./src/istio/ingress-virtual-service-yaml` file

    # See your mesh metrics
    
    1. Install Kiali and Prometheus
    ```
    kubectl apply -f ${ISTIO_HOME}/samples/addons/kiali.yaml
    kubectl apply -f ${ISTIO_HOME}/samples/addons/prometheus.yaml
    ```
    2. Open Kiali and play a little bit:
    ```
    istioctl dashboard kiali
    ```
    3. You may also try Jeager and Grafana:
    ```
    kubectl apply -f ${ISTIO_HOME}/samples/addons/jaeger.yaml
    kubectl apply -f ${ISTIO_HOME}/samples/addons/grafana.yaml
    ```
    (You can open each dashboard using `istioctl dashboard <service>`)

    # Advanced routing on the Istio Ingress Gateway
