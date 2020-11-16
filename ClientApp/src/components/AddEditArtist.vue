<template>
  <section id="app" class="section">
      <h1 class="title is-1">
          Add/Edit Artist
      </h1>

      <div class="columns">
          <div class="column">
              <form>
                  <div class="field">
                      <label class="label">Input Artist Name To Search</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.userName" />
                      </div>
                      <div class="control">
                           <input class="button is-primary" type="submit" value="Get Artist"/>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Favorite Color</label>
                      <div class="control">
                          <div class="select">
                              <select v-model="form.favoriteColor">
                                  <option v-for="color in ['Red', 'Blue', 'Green']" :key="color" :value="color">
                                      {{color}}
                                  </option>
                              </select>
                          </div>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Artist Name</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.userName" />
                      </div>
                  </div>

                  <input class="button is-primary margin-bottom" type="submit" @click.prevent="fakeSubmit" />

              </form>

              <transition name="fade" mode="out-in">
                  <article class="message is-primary" v-show="showSubmitFeedback">
                      <div class="message-header">
                          <p>Fake Send Status:</p>
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
            formName: "Tell Us About Yourself",
            userName: "",
            favoriteColor: "Red",
            favoriteHamburger: "",
            favoriteHangout: [],
            workHours: 0
        },
        showSubmitFeedback: false
    };
  },
  computed: {
    datasets() {
      if (!this.data) return null;
      return this.data.map((d) => {
        return {
          slug: d.account,
          data: d.streamInfos.map((i) => {
            return { value: i.streams, day: i.day };
          }),
        };
      });
    },
  },
  methods: {
    onChange() {
      console.log(this.selected);
      this.$http
        .get(`http://139.180.139.12/api/accountstream/last30days?name=` + this.selected)
        .then((res) => {
          this.data = res.data;
        })
        .catch((error) => console.log(error));
    },
    fakeSubmit() {
            this.showSubmitFeedback = true;
            setTimeout(() => {
                this.showSubmitFeedback = false;
            }, 3000);
        }
  },
  mounted() {
    
  },
};
</script>

<style lang="scss">
.margin-bottom {
  margin-bottom: 15px;
}

.fade-enter, .fade-leave-active {
  opacity: 0;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
</style>
