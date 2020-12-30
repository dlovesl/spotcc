import Api from "./api";

export default {
  login(params) {
    return Api().post("/auth/login?token="+params);
  },

  logout() {
    localStorage.removeItem("user");
  },

  isAuthenticated() {
    let user = JSON.parse(localStorage.getItem("user"));
    return !!user;
  }
};
