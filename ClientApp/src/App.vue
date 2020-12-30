<template>
  <div style="height:100%;">
    <router-view v-if="!isAuthenticated"/>
    <section v-if="isAuthenticated" class="demo">
      <header class="demo__header">
        <h1>Spot Trend Chart</h1>
      </header>
      <div class="ht-nagivation">
        <router-link to="/stats">Stats</router-link>
        <span class="ht-icon">|</span>
        <router-link to="/pre">Summary</router-link>
        <span class="ht-icon">|</span>
        <router-link to="/artist">Manage Artist</router-link>
      </div>
      <main class="demo__examples">
        <div class="demo__example">
          <div class="demo__example-container">
            <router-view/>
          </div>
        </div>
      </main>
      <footer class="demo__footer">
        Released under the SpotCC team license.
      </footer>
    </section>
  </div>  
</template>

<script>
import authService from "./services/authentication";

export default {
  name: "app",
  data() {
    return {
      isAuthenticated: false
    }
  },
  mounted() {
    this.isAuthenticated = authService.isAuthenticated();
  },  
  watch: {
    '$route' (to, from) {
      if(from.path === '/login'){
        this.isAuthenticated = true;
        let user = JSON.parse(localStorage.getItem("user"));
        if(user.message) {
          this.$buefy.notification.open({
                    duration: 5000,
                    message: user.message,
                    type: 'is-warning',
                    hasIcon: true
                });
        }
      } else if (to.path === '/login'){
        this.isAuthenticated = false;
      }
    }
  }
};
</script>

<style lang="scss">
@import url("https://fonts.googleapis.com/css?family=Montserrat:400|Source+Sans+Pro:400,600");
* {
  box-sizing: border-box;
}
body {
  padding: 0;
  margin: 0;
  font-family: "Source Sans Pro", sans-serif;
  color: #2f4053;
}
.demo {
  margin: 0 auto;
  background: #fff;
  a {
    color: #39af77;
  }
  &__header {
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 150px;
    padding: 20px 20px 40px;
    @media (min-width: 768px) {
      height: 200px;
      padding: 20px 20px 50px;
    }
  }
  &__logo {
    width: auto;
    height: 60px;
  }
  &__examples {
    width: 100%;
  }
  &__example {
    position: relative;
    padding: 40px 20px;
    &:first-child {
      padding-top: 30px;
    }
    &:nth-child(odd) {
      background: #fbfbfb;
    }
    @media (min-width: 768px) {
      padding: 80px 20px;
      &:first-child {
        padding-top: 50px;
      }
    }
    &-container {
      width: 100%;
      max-width: 1000px;
      margin: 0 auto;
    }
    &-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
      padding-bottom: 10px;
      margin-bottom: 20px;
      border-bottom: 1px solid rgba(#333, 0.1);
    }
    &-intro {
      width: 100%;
      padding-right: 20px;
    }
    &-heading {
      display: block;
      font-size: 18px;
    }
    &-description {
      font-size: 14px;
      opacity: 0.5;
    }
  }
  &__show-code {
    display: inline-block;
    padding: 0 10px;
    flex-shrink: 0;
    text-transform: uppercase;
    text-decoration: none;
    font-size: 12px;
    line-height: 24px;
    border: 1px solid #39af77;
    transition: all 0.5s;
    &:hover {
      color: #fff;
      background: #39af77;
    }
  }
  &__footer {
    padding: 30px 20px;
    font-size: 13px;
    text-align: center;
    background: #fbfbfb;
  }
}
.wave {
  width: 100%;
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
}
.ht-nagivation{
  display: flex;
  align-items: center;
  justify-content: center;
}
.ht-nagivation .ht-icon {
  padding-left: 20px;
  padding-right: 20px;
}
.demo__header{
  font-size: 50px;
}
</style>
