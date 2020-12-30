import axios from "axios";
import { router } from "../routes";

let baseUrl = "http://78.141.232.110/spotcc/api";

export default () => {
  var instance = axios.create({
    baseURL: baseUrl,
    withCredentials: false,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
  });

  instance.interceptors.response.use(
    (response) => {
      return Promise.resolve(response);
    },
    (error) => {
      const { status } = error.response;
      if (status === 404 || status === 403) {
        router.push("/login");
      }

      return Promise.reject(error);
    }
  );

  return instance;
};
