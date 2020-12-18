<template>
  <section id="app" class="section">
      <h1 class="title is-1">
          Edit Account
      </h1>

      <div class="columns">
          <div class="column">
              <form>
                  <div class="field">
                      <label class="label">Account</label>
                      <div class="control">
                          <div class="select">
                              <select v-model="form.name" @change="onChange()">
                                  <option disabled value="">Please select one</option>
                                  <option v-for="acc in accounts" :key="acc.name" :value="acc.name">
                                      {{acc.name}}
                                  </option>
                              </select>
                          </div>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Total Pre Streams</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.orderPreStreams" />
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Total Free Streams</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.ordeFreeStreams" />
                      </div>
                  </div>

                  <input class="button is-primary margin-bottom" type="submit" @click.prevent="update" value="Update"/>
              </form>

              <transition name="fade" mode="out-in">
                  <article class="message is-primary" v-show="showSubmitFeedback">
                      <div class="message-header">
                          <p>Send Status:</p>
                      </div>
                      <div class="message-body">
                          Successfully Submitted!
                      </div>
                  </article>
              </transition>
          </div>
          <div class="column">
              <h5>
                  JSON
              </h5>
              <pre><code>{{form}}</code></pre>
          </div>
      </div>

  </section>
</template>

<script>

import "vue-select/dist/vue-select.css";

export default {
  components: {  },
  data() {
    return {
      form: {
            orderPreStreams: "",
            ordeFreeStreams: "",
            name: ""
        },
      showSubmitFeedback: false,
      accounts: [],
    };
  },
  computed: {

  },
  methods: {
    update() {
        this.$http
        .post(`http://78.141.232.110/lq1ss/api/account/update`, this.form)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
        }, 3000);
        })
        .catch((error) => console.log(error));
    },
    onChange() {
      if(this.form.name == '') return;

      this.$http
        .get(`http://78.141.232.110/lq1ss/api/account/` + this.form.name)
        .then((res) => {
          console.log(res.data);
          this.form.name = res.data.name;
          this.form.ordeFreeStreams = res.data.ordeFreeStreams;
          this.form.orderPreStreams = res.data.orderPreStreams;
        })
        .catch((error) => console.log(error));
    },
    fetchData() {
      this.$http
        .get(`http://78.141.232.110/lq1ss/api/account/getall`)
        .then((res) => {
          this.accounts = res.data;
        })
        .catch((error) => console.log(error));
    },
  },
  mounted() {
    this.fetchData();
  },
};
</script>

<style lang="scss">
.margin-bottom {
  margin-bottom: 15px;
}

.margin-left {
  margin-left: 15px;
}

.fade-enter, .fade-leave-active {
  opacity: 0;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
</style>
