<template>
  <section id="app" class="section">
      <div class="columns">
          <div class="column">
              <form class="margin-bottom">
                <b-field grouped label="Total steam you bought" :label-position="labelPosition">
                  <b-input placeholder="total stream..." v-model="form.totalStream"></b-input>
                  <p class="control">
                      <button class="button is-primary"  @click.prevent="accountUpdate">Update</button>
                  </p>
                </b-field>
                <b-field grouped>
                  <b-field grouped label="Artist Name" :label-position="labelPosition">
                    <b-input type="text" v-model="form.name"></b-input>
                  </b-field>

                  <b-field grouped label="Artist Id" :label-position="labelPosition">
                      <b-input type="text" v-model="form.spotifyId"></b-input>
                  </b-field>

                  <input class="button is-primary margin-bottom margin-left" type="submit" @click.prevent="add" value="Add"/>
                </b-field>
                <div class="columns">
                  <div class="column">
                    <label class="label">Your artist list: </label>
                    <select class="lstbox"
                            size="8"
                            v-model="form.artistId">
                            <option v-for="acc in artists" :key="acc.spotifyId" :value="acc.spotifyId">
                                {{acc.name}}
                            </option>
                    </select>
                  </div>
                  <div class="column center-btn">
                      <input class="button is-primary margin-bottom margin-left" type="submit" @click.prevent="update" value="Remove"/>
                  </div>
                </div>
               
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
            totalStream: "",
            name: "",
            spotifyId: "",
            artistId: "",
            streamType: 0,
            id:0
        },
      showSubmitFeedback: false,
      artists: [],
      labelPosition: 'on-border'
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
    accountUpdate() {
      if(this.form.totalStream < 1) return;
        this.$http
        .post(`http://78.141.232.110/lq1ss/api/account/updateStream?stream=` + this.form.totalStream)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
        }, 3000);
        })
        .catch((error) => console.log(error));
    },
    add() {
      if(this.form.name == '' || this.form.spotifyId == '') return;
        this.form.id = 0;
        this.$http
        .post(`http://78.141.232.110/lq1ss/api/artist/create`, this.form)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
            this.updateList();
        }, 3000);
        })
        .catch((error) => console.log(error));
    },
    update() {
      console.log(this.form.artistId);
      if(this.form.artistId == '') return;
        this.$http
        .post(`http://78.141.232.110/lq1ss/api/artist/remove/` + this.form.artistId)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
            this.form.artistId = "";
            this.updateList();
        }, 3000);
        })
        .catch((error) => console.log(error));
    },

    updateList() {
      this.$http
        .get(`http://78.141.232.110/lq1ss/api/artist/getactive`)
        .then((res) => {
          this.artists = res.data;
        })
        .catch((error) => console.log(error));
    },

    fetchData() {
      this.updateList();
    },
  },
  mounted() {
    this.fetchData();
  },
};
</script>

<style lang="scss">
.column .center-btn {
  display: flex;
  align-items: center;
}

.lstbox {
  width: 100% ;
}

.lstbox option {
    font-weight: normal;
    display: block;
    white-space: pre;
    min-height: 1.2em;
    padding: 0.5em 1em;
    font-size: 1rem;
}

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
