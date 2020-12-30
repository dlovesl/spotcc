<template>
<div id="login-section">
  <div id="login">
    <div id="form">
      <form @submit.prevent="login">
        <label for="email">TOKEN</label>
        <input type="text" id="token" v-model="token" placeholder="input your token">
        <div style="text-align:center;">
          <button type="submit">Log in</button>
        </div>
      </form>
    </div>
  </div>
</div>
</template>

<script src="https://cdn.jsdelivr.net/npm/vue"></script>

<script>
import moment from 'moment';
import "vue-select/dist/vue-select.css";
import authService from "../services/authentication";

export default {
  components: {  },
  data() {
    return {
      token: ''
    };
  },
  methods: {
    login() {
      authService.login(this.token)
            .then(res => {
              localStorage.setItem("user", JSON.stringify(res.data));
              this.$router.push({ path: "/" });
            })
            .catch(err => {
              this.$buefy.toast.open({
                    message: 'Token is incorrect, please try again!',
                    type: 'is-danger'
                });              
            });
    }
  }
};
  
</script>

<style lang="scss">
@import url("https://fonts.googleapis.com/css?family=Nunito");
@import url("https://use.fontawesome.com/releases/v5.0.6/css/all.css");

* {
  box-sizing: border-box;
  font-family: 'Nunito', sans-serif;
}

html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
  width: 100%;
}

div#login-section {
  width: 100%;
  height: 100%;
}

div#login-section div#login {
  align-items: center;
  background-color: #e2e2e5;
  display: flex;
  justify-content: center;
  width: 100%;
  height: 100%;
}

div#login-section div#login div#description {
  background-color: #ffffff;
  width: 280px;
  padding: 35px;
}

div#login-section div#login div#description h1,
div#login-section div#login div#description p {
  margin: 0;
}

div#login-section div#login div#description p {
  font-size: 0.8em;
  color: #95a5a6;
  margin-top: 10px;
}

div#login-section div#login div#form {
  background-color: #34495e;
  border-radius: 5px;
  box-shadow: 0px 0px 30px 0px #666;
  color: #ecf0f1;
  width: 500px;
  padding: 35px;
}

div#login-section div#login div#form label,
div#login-section div#login div#form input {
  outline: none;
  width: 100%;
}

div#login-section div#login div#form label {
  color: #95a5a6;
  font-size: 0.8em;
}

div#login-section div#login div#form input {
  background-color: #8c96a0;
  border: none;
  color: #ecf0f1;
  font-size: 1em;
  margin-bottom: 20px;
}

div#login-section div#login div#form ::placeholder {
  color: #ecf0f1;
  opacity: 1;
}

div#login-section div#login div#form button {
  background-color: #ffffff;
  cursor: pointer;
  border: none;
  padding: 10px;
  transition: background-color 0.2s ease-in-out;
  width: 30%;
}

div#login-section div#login div#form button:hover {
  background-color: #eeeeee;
}

@media screen and (max-width: 600px) {
  div#login-section div#login {
    align-items: unset;
    background-color: unset;
    display: unset;
    justify-content: unset;
  }

  div#login-section div#login div#description {
    margin: 0 auto;
    max-width: 350px;
    width: 100%;
  }

  div#login-section div#login div#form {
    border-radius: unset;
    box-shadow: unset;
    width: 100%;
  }

  div#login-section div#login div#form form {
    margin: 0 auto;
    max-width: 280px;
    width: 100%;
  }
}
</style>

